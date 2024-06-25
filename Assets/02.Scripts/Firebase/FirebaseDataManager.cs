using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnumTypes;
using EventLibrary;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseDataManager : Singleton<FirebaseDataManager>
{
    public DatabaseReference DatabaseReference => _databaseRef; // 데이터베이스 참조
    
    private DatabaseReference _databaseRef;
    private FirebaseAuth _auth;
    private FirebaseUser currentUser;
    private Logger logger;

    protected override void Awake()
    {
        base.Awake();
        
        EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseInitialized, OnFirebaseInitialized);
        EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);

        logger = Logger.Instance;
    }

    private void OnDestroy()
    {
        EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseInitialized, OnFirebaseInitialized);
        EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
    }

    // Firebase가 초기화될 때 호출
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
    }

    // 현재 Firebase 사용자를 반환
    public FirebaseUser GetCurrentUser() 
    {
        return currentUser;
    }

    // FirebaseUser 데이터를 저장
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

    // UserData 객체를 Firebase 데이터베이스에 저장
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

    // UserHeroCollection 객체를 Firebase 데이터베이스에 저장
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
    }

    // Firebase 데이터베이스에서 사용자 데이터를 로드
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
                var userHeroCollection = new UserHeroCollection(userId, HeroCollectionManager.Instance.ToBase64());

                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터 불러오기 성공: " + userData.displayName + ", " + userData.level + ", " + userData.items));
                // 여기서 userData와 userHeroCollection을 필요한 곳에 사용
            }
            else
            {
                UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 데이터가 존재하지 않습니다."));
            }
        });
    }
    
    // user_HeroCollection만 로드
    public async Task<UserHeroCollection> LoadHeroCollection(string userId)
    {
        var heroDataSnapshot = await _databaseRef.Child("user_HeroCollection").Child(userId).GetValueAsync();

        if (heroDataSnapshot.Exists)
        {
            string base64HeroCollection = heroDataSnapshot.Child("heroCollection").Value.ToString();
            var userHeroCollection = new UserHeroCollection(userId, base64HeroCollection);
            return userHeroCollection;
        }
        else
        {
            UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 히어로 컬렉션 데이터가 존재하지 않습니다."));
            return null;
        }
    }

    // 사용자 데이터를 초기화
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
    }

    // 사용자 데이터를 동기화
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

    // 모든 데이터를 삭제
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
    }

    // 유저가 새로운 영웅을 획득할 때 호출되는 메서드
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
        }
        else
        {
            UnityMainThreadDispatcher.Enqueue(() => logger.Log("유저 히어로 컬렉션 데이터가 존재하지 않습니다."));
        }
    }
}
