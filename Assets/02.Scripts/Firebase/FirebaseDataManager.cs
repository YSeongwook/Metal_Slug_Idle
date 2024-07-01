using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnumTypes;
using EventLibrary;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Text;

public class FirebaseDataManager : Singleton<FirebaseDataManager>
{
    private DatabaseReference _databaseRef;
    private FirebaseAuth _auth;
    private FirebaseUser currentUser;
    private Logger logger;

    protected override void Awake()
    {
        base.Awake();
        
        EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseInitialized, OnFirebaseInitialized);
        EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);

        // HeroCollectionUpdated 이벤트 리스너 등록
        UnityAction<HeroCollection> onHeroCollectionUpdated = OnHeroCollectionUpdated;
        EventManager<DataEvents>.StartListening(DataEvents.HeroCollectionUpdated, onHeroCollectionUpdated);

        logger = Logger.Instance;
    }

    private void OnDestroy()
    {
        EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseInitialized, OnFirebaseInitialized);
        EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);

        // HeroCollectionUpdated 이벤트 리스너 해제
        UnityAction<HeroCollection> onHeroCollectionUpdated = OnHeroCollectionUpdated;
        EventManager<DataEvents>.StopListening(DataEvents.HeroCollectionUpdated, onHeroCollectionUpdated);
    }

    private void OnFirebaseInitialized() 
    {
        _auth = FirebaseAuth.DefaultInstance;
        _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        logger = Logger.Instance; // Logger 인스턴스 초기화
        
        UnityMainThreadDispatcher.Enqueue(() => {
            logger.Log($"Realtime Database: {_databaseRef}");
            logger.Log($"Auth: {_auth}");
        });
        
        EventManager<FirebaseEvents>.TriggerEvent(FirebaseEvents.FirebaseDatabaseInitialized);
    }

    private void OnFirebaseSignIn() 
    {
        currentUser = AuthManager.Instance.GetCurrentUser();
        SaveUserData(currentUser);
        LoadHeroDataFromFirebase(heroData => {
            // Firebase에서 불러온 데이터를 GachaManager에 전달합니다.
            GachaManager.Instance.SetHeroData(heroData);
        });
    }

    public FirebaseUser GetCurrentUser() 
    {
        return currentUser;
    }

    private async void SaveUserData(FirebaseUser user)
    {
        var userId = user.UserId;
    
        var userDataSnapshot = await _databaseRef.Child("users").Child(userId).GetValueAsync();
        var userHeroCollectionSnapshot = await _databaseRef.Child("user_HeroCollection").Child(userId).GetValueAsync();
    
        if (!userDataSnapshot.Exists || !userHeroCollectionSnapshot.Exists)
        {
            HeroCollectionManager.Instance.Initialize(30);
            var userData = new UserData(user.UserId, user.DisplayName ?? "None", 1, "None", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            var userHeroCollection = new UserHeroCollection(user.UserId, HeroCollectionManager.Instance.ToBase64());
        
            await SaveUserData(userData);
            await SaveHeroCollection(userHeroCollection);
        }
        else
        {
            UnityMainThreadDispatcher.Enqueue(() => logger.Log("기존 사용자 데이터가 존재합니다."));
        }
    }

    private async Task SaveUserData(UserData userData)
    {
        var updates = new Dictionary<string, object>
        {
            { "displayName", userData.displayName },
            { "level", userData.level },
            { "items", userData.items },
            { "lastUpdated", userData.lastUpdated }
        };

        await _databaseRef.Child("users").Child(userData.userId).UpdateChildrenAsync(updates).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터 저장이 취소되었습니다."));
                return;
            }
            if (task.IsFaulted)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.LogError("유저 데이터 저장 중 오류 발생: " + task.Exception));
                return;
            }

            UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터 저장 성공."));
        });
    }

    private async Task SaveHeroCollection(UserHeroCollection userHeroCollection)
    {
        var updates = new Dictionary<string, object>
        {
            { "heroCollection", userHeroCollection.heroCollectionBase64 }
        };

        await _databaseRef.Child("user_HeroCollection").Child(userHeroCollection.userId).UpdateChildrenAsync(updates).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 히어로 컬렉션 저장이 취소되었습니다."));
                return;
            }
            if (task.IsFaulted)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.LogError("유저 히어로 컬렉션 저장 중 오류 발생: " + task.Exception));
                return;
            }

            UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 히어로 컬렉션 저장 성공."));
        });

        // 로컬 파일 업데이트
        SaveHeroCollectionToLocalFile(userHeroCollection.heroCollectionBase64);
    }

    private void OnHeroCollectionUpdated(HeroCollection heroCollection)
    {
        string userId = GetCurrentUser().UserId;
        string base64HeroCollection = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(heroCollection)));

        _databaseRef.Child("user_HeroCollection").Child(userId).Child("heroCollection").SetRawJsonValueAsync(base64HeroCollection)
            .ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("HeroCollection updated in Firebase Realtime Database.");
                }
                else
                {
                    Debug.LogError("Failed to update HeroCollection in Firebase Realtime Database: " + task.Exception);
                }
            });

        // 로컬 파일 업데이트
        SaveHeroCollectionToLocalFile(base64HeroCollection);
    }

    private void SaveHeroCollectionToLocalFile(string base64HeroCollection)
    {
        string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64HeroCollection));
        string path = Path.Combine(Application.persistentDataPath, "HeroCollection.json");
        File.WriteAllText(path, json);
        Debug.Log("HeroCollection saved to local file.");
    }

    public async void LoadUserData(string userId)
    {
        var userDataTask = _databaseRef.Child("users").Child(userId).GetValueAsync();
        var heroDataTask = _databaseRef.Child("user_HeroCollection").Child(userId).GetValueAsync();

        await Task.WhenAll(userDataTask, heroDataTask).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터 불러오기가 취소되었습니다."));
                return;
            }
            if (task.IsFaulted)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.LogError("유저 데이터 불러오기 중 오류 발생: " + task.Exception));
                return;
            }

            DataSnapshot userDataSnapshot = userDataTask.Result;
            DataSnapshot heroDataSnapshot = heroDataTask.Result;

            if (userDataSnapshot.Exists && heroDataSnapshot.Exists)
            {
                UserData userData = new UserData(
                    userId,
                    userDataSnapshot.Child("displayName").Value.ToString(),
                    int.Parse(userDataSnapshot.Child("level").Value.ToString()),
                    userDataSnapshot.Child("items").Value.ToString(),
                    long.Parse(userDataSnapshot.Child("lastUpdated").Value.ToString())
                );

                string base64HeroCollection = heroDataSnapshot.Child("heroCollection").Value.ToString();
                HeroCollectionManager.Instance.FromBase64(base64HeroCollection);
                var userHeroCollection = new UserHeroCollection(userId, base64HeroCollection);

                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터 불러오기 성공: " + userData.displayName + ", " + userData.level + ", " + userData.items));

                // 로컬 파일 업데이트
                SaveHeroCollectionToLocalFile(base64HeroCollection);
            }
            else
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터가 존재하지 않습니다."));
            }
        });
    }

    public async Task<UserHeroCollection> LoadHeroCollection(string userId)
    {
        var heroDataSnapshot = await _databaseRef.Child("user_HeroCollection").Child(userId).GetValueAsync();

        if (heroDataSnapshot.Exists)
        {
            string base64HeroCollection = heroDataSnapshot.Child("heroCollection").Value.ToString();
            var userHeroCollection = new UserHeroCollection(userId, base64HeroCollection);
            HeroCollectionManager.Instance.FromBase64(base64HeroCollection);

            // 로컬 파일 업데이트
            SaveHeroCollectionToLocalFile(base64HeroCollection);

            return userHeroCollection;
        }
        else
        {
            UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 히어로 컬렉션 데이터가 존재하지 않습니다."));
            return null;
        }
    }

    public void ResetUserData(string userId)
    {
        _databaseRef.Child("users").Child(userId).RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터 리셋이 취소되었습니다."));
                return;
            }
            if (task.IsFaulted)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.LogError("유저 데이터 리셋 중 오류 발생: " + task.Exception));
                return;
            }

            UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터 리셋 성공."));
        });

        _databaseRef.Child("user_HeroCollection").Child(userId).RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 히어로 컬렉션 리셋이 취소되었습니다."));
                return;
            }
            if (task.IsFaulted)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.LogError("유저 히어로 컬렉션 리셋 중 오류 발생: " + task.Exception));
                return;
            }

            UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 히어로 컬렉션 리셋 성공."));
        });

        // 로컬 파일 삭제
        DeleteLocalHeroCollectionFile();
    }

    private void DeleteLocalHeroCollectionFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "HeroCollection.json");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("HeroCollection local file deleted.");
        }
    }

    public void SyncUserData(string userId)
    {
        _databaseRef.Child("users").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터 불러오기가 취소되었습니다."));
                return;
            }
            if (task.IsFaulted)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.LogError("유저 데이터 불러오기 중 오류 발생: " + task.Exception));
                return;
            }

            DataSnapshot snapshot = task.Result;
            UserData serverData = snapshot.Exists ? JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue()) : null;
            UserData localData = JsonUtilityManager.LoadFromJson<UserData>("UserData.json");

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                if (serverData == null)
                {
                    if (localData != null)
                    {
                        SaveUserData(localData);
                        logger.Log("로컬 데이터를 서버에 저장했습니다.");
                    }
                    else
                    {
                        logger.Log("서버와 로컬에 모두 유저 데이터가 없습니다.");
                    }
                }
                else
                {
                    if (localData == null || serverData.lastUpdated > localData.lastUpdated)
                    {
                        JsonUtilityManager.SaveToJson(serverData, "UserData.json");
                        logger.Log("서버 데이터로 로컬 데이터를 업데이트했습니다.");
                    }
                    else if (localData.lastUpdated > serverData.lastUpdated)
                    {
                        SaveUserData(localData);
                        logger.Log("로컬 데이터로 서버 데이터를 업데이트했습니다.");
                    }
                    else
                    {
                        logger.Log("서버와 로컬 데이터가 이미 동기화되어 있습니다.");
                    }
                }
            });
        });
    }

    public void DeleteAllData()
    {
        _databaseRef.RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.Log("모든 데이터 삭제가 취소되었습니다."));
                return;
            }
            if (task.IsFaulted)
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.LogError("모든 데이터 삭제 중 오류 발생: " + task.Exception));
                return;
            }

            UnityMainThreadDispatcher.Enqueue(() => logger.Log("모든 데이터 삭제 성공."));
        });

        // 로컬 파일 삭제
        DeleteLocalHeroCollectionFile();
    }

    public async void AddHeroToCollection(string userId, int heroId)
    {
        var heroDataSnapshot = await _databaseRef.Child("user_HeroCollection").Child(userId).GetValueAsync();
        
        if (heroDataSnapshot.Exists)
        {
            string base64HeroCollection = heroDataSnapshot.Child("heroCollection").Value.ToString();
            HeroCollectionManager.Instance.FromBase64(base64HeroCollection);
            
            HeroCollectionManager.Instance.AddHero(heroId);
            
            var userHeroCollection = new UserHeroCollection(userId, HeroCollectionManager.Instance.ToBase64());
            await SaveHeroCollection(userHeroCollection);

            // 로컬 파일 업데이트
            SaveHeroCollectionToLocalFile(userHeroCollection.heroCollectionBase64);
        }
        else
        {
            UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 히어로 컬렉션 데이터가 존재하지 않습니다."));
        }
    }
    
    public async void LoadHeroDataFromFirebase(Action<HeroDataWrapper> callback)
    {
        if (_databaseRef == null || currentUser == null)
        {
            UnityMainThreadDispatcher.Enqueue(() => logger.LogError("Firebase Database or User not initialized."));
            return;
        }

        var heroDataSnapshot = await _databaseRef.Child("HeroData").GetValueAsync();

        if (heroDataSnapshot.Exists)
        {
            var heroDataJson = heroDataSnapshot.GetRawJsonValue();
            var heroDataWrapper = JsonUtility.FromJson<HeroDataWrapper>(heroDataJson);
            callback(heroDataWrapper);
        }
        else
        {
            UnityMainThreadDispatcher.Enqueue(() => logger.Log("HeroData not found in Firebase Realtime Database."));
        }
    }

    public async Task UpdateHeroCollection(int[] heroIds)
    {
        string userId = GetCurrentUser().UserId;
        var heroDataSnapshot = await _databaseRef.Child("user_HeroCollection").Child(userId).GetValueAsync();

        if (heroDataSnapshot.Exists)
        {
            string base64HeroCollection = heroDataSnapshot.Child("heroCollection").Value.ToString();
            HeroCollectionManager.Instance.FromBase64(base64HeroCollection);

            foreach (int heroId in heroIds)
            {
                HeroCollectionManager.Instance.AddHero(heroId);
            }

            var userHeroCollection = new UserHeroCollection(userId, HeroCollectionManager.Instance.ToBase64());
            await SaveHeroCollection(userHeroCollection);

            // 로컬 파일 업데이트
            SaveHeroCollectionToLocalFile(userHeroCollection.heroCollectionBase64);
        }
        else
        {
            UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 히어로 컬렉션 데이터가 존재하지 않습니다."));
        }
    }
}
