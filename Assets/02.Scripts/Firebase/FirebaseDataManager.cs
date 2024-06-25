using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using EnumTypes;
using EventLibrary;
using Firebase.Auth;
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
        
        // 이벤트 리스너 등록
        EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseInitialized, OnFirebaseInitialized);
        EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);

        logger = Logger.Instance;
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 해제
        EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseInitialized, OnFirebaseInitialized);
        EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
    }

    private void OnFirebaseInitialized()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        logger = Logger.Instance; // Logger 인스턴스 초기화
        logger.Log($"Realtime Database: {_databaseRef}");
        logger.Log($"Auth: {_auth}");
        
        EventManager<FirebaseEvents>.TriggerEvent(FirebaseEvents.FirebaseDatabaseInitialized);
    }

    private void OnFirebaseSignIn()
    {
        currentUser = AuthManager.Instance.GetCurrentUser();
        
        SaveUserData(currentUser);
    }

    public FirebaseUser GetCurrentUser()
    {
        return currentUser;
    }

    // FirebaseUser를 받아 저장하는 메서드
    private async void SaveUserData(FirebaseUser user)
    {
        byte[] heroCollection = new byte[30]; // 초기값, 실제 게임 로직에 따라 변경
        var userData = new UserData(user.UserId, user.DisplayName ?? "None", 1, "None", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var userHeroCollection = new UserHeroCollection(user.UserId, heroCollection);
        
        await SaveUserData(userData);
        await SaveHeroCollection(userHeroCollection);
    }

    // UserData를 받아 저장하는 메서드
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
                logger.Log("유저 데이터 저장이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                logger.LogError("유저 데이터 저장 중 오류 발생: " + task.Exception);
                return;
            }

            logger.Log("유저 데이터 저장 성공.");
        });
    }

    // UserHeroCollection을 받아 저장하는 메서드
    private async Task SaveHeroCollection(UserHeroCollection userHeroCollection)
    {
        byte[] compressedHeroCollection = await CompressionUtility.Compress(Convert.ToBase64String(userHeroCollection.heroCollection));
        string base64HeroCollection = Convert.ToBase64String(compressedHeroCollection);
        var updates = new Dictionary<string, object>
        {
            { "heroCollection", base64HeroCollection }
        };

        await _databaseRef.Child("user_HeroCollection").Child(userHeroCollection.userId).UpdateChildrenAsync(updates).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                logger.Log("유저 히어로 컬렉션 저장이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                logger.LogError("유저 히어로 컬렉션 저장 중 오류 발생: " + task.Exception);
                return;
            }

            logger.Log("유저 히어로 컬렉션 저장 성공.");
        });
    }

    public async void LoadUserData(string userId)
    {
        var userDataTask = _databaseRef.Child("users").Child(userId).GetValueAsync();
        var heroDataTask = _databaseRef.Child("user_HeroCollection").Child(userId).GetValueAsync();

        await Task.WhenAll(userDataTask, heroDataTask).ContinueWith(async task =>
        {
            if (task.IsCanceled)
            {
                logger.Log("유저 데이터 불러오기가 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                logger.LogError("유저 데이터 불러오기 중 오류 발생: " + task.Exception);
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
                byte[] compressedHeroCollection = Convert.FromBase64String(base64HeroCollection);
                string decodedHeroCollection = await CompressionUtility.Decompress(compressedHeroCollection);
                byte[] heroCollection = Convert.FromBase64String(decodedHeroCollection);
                var userHeroCollection = new UserHeroCollection(userId, heroCollection);

                logger.Log("유저 데이터 불러오기 성공: " + userData.displayName + ", " + userData.level + ", " + userData.items);
                // 여기서 userData와 userHeroCollection을 필요한 곳에 사용하면 됩니다.
            }
            else
            {
                logger.Log("유저 데이터가 존재하지 않습니다.");
            }
        });
    }

    public void ResetUserData(string userId)
    {
        _databaseRef.Child("users").Child(userId).RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                logger.Log("유저 데이터 리셋이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                logger.LogError("유저 데이터 리셋 중 오류 발생: " + task.Exception);
                return;
            }

            logger.Log("유저 데이터 리셋 성공.");
        });

        _databaseRef.Child("user_HeroCollection").Child(userId).RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                logger.Log("유저 히어로 컬렉션 리셋이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                logger.LogError("유저 히어로 컬렉션 리셋 중 오류 발생: " + task.Exception);
                return;
            }

            logger.Log("유저 히어로 컬렉션 리셋 성공.");
        });
    }

    public void SyncUserData(string userId)
    {
        _databaseRef.Child("users").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                logger.Log("유저 데이터 불러오기가 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                logger.LogError("유저 데이터 불러오기 중 오류 발생: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            UserData serverData = snapshot.Exists ? JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue()) : null;
            UserData localData = JsonUtilityManager.LoadFromJson<UserData>("UserData.json");

            if (serverData == null)
            {
                if (localData != null)
                {
                    // 서버에 데이터가 없고 로컬에 데이터가 있는 경우 로컬 데이터를 서버에 저장
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
                    // 서버 데이터가 최신이거나 로컬 데이터가 없는 경우 서버 데이터로 로컬 데이터를 업데이트
                    JsonUtilityManager.SaveToJson(serverData, "UserData.json");
                    logger.Log("서버 데이터로 로컬 데이터를 업데이트했습니다.");
                }
                else if (localData.lastUpdated > serverData.lastUpdated)
                {
                    // 로컬 데이터가 최신인 경우 로컬 데이터를 서버에 저장
                    SaveUserData(localData);
                    logger.Log("로컬 데이터로 서버 데이터를 업데이트했습니다.");
                }
                else
                {
                    logger.Log("서버와 로컬 데이터가 이미 동기화되어 있습니다.");
                }
            }
        });
    }

    public void DeleteAllData()
    {
        _databaseRef.RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                logger.Log("모든 데이터 삭제가 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                logger.LogError("모든 데이터 삭제 중 오류 발생: " + task.Exception);
                return;
            }

            logger.Log("모든 데이터 삭제 성공.");
        });
    }
}
