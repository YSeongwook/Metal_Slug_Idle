using Firebase;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseDataManager : Singleton<FirebaseDataManager>
{
    private DatabaseReference _databaseRef;
    private FirebaseAuth _auth;
    public Logger logger;
    public DatabaseReference DatabaseReference => _databaseRef; // 데이터베이스 참조

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                logger.LogError("Firebase 초기화 실패: " + task.Exception);
                return;
            }

            FirebaseApp app = FirebaseApp.DefaultInstance; // Firebase 앱 인스턴스 가져오기
            _auth = FirebaseAuth.DefaultInstance; // Firebase 인증 인스턴스 가져오기
            _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            logger.Log($"Firebase 초기화 완료: {_databaseRef}");
            logger.Log($"Auth: {_auth}");
        });
    }

    public void SaveUserData(FirebaseUser user)
    {
        User userData = new User(user.UserId, user.DisplayName, 1, "None");
        string json = JsonUtility.ToJson(userData);
        _databaseRef.Child("users").Child(user.UserId).SetRawJsonValueAsync(json).ContinueWith(task =>
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

    public void LoadUserData(string userId)
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
            User userData = JsonUtility.FromJson<User>(snapshot.GetRawJsonValue());
            logger.Log("유저 데이터 불러오기 성공: " + userData.displayName + ", " + userData.level + ", " + userData.items);
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
            UserData serverData = JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue());
            UserData localData = JsonUtilityManager.LoadFromJson<UserData>("UserData.json");

            if (localData == null || serverData.lastUpdated > localData.lastUpdated)
            {
                // 서버 데이터가 더 최신이므로 로컬 데이터를 서버 데이터로 덮어쓰기
                JsonUtilityManager.SaveToJson(serverData, "UserData.json");
                logger.Log("서버 데이터로 로컬 데이터를 업데이트했습니다.");
            }
            else if (localData.lastUpdated > serverData.lastUpdated)
            {
                // 로컬 데이터가 더 최신이므로 서버 데이터를 로컬 데이터로 업데이트
                SaveUserDataToServer(localData);
                logger.Log("로컬 데이터로 서버 데이터를 업데이트했습니다.");
            }
        });
    }

    public void SaveUserDataToServer(UserData userData)
    {
        string json = JsonUtility.ToJson(userData);
        _databaseRef.Child("users").Child(userData.userId).SetRawJsonValueAsync(json).ContinueWith(task =>
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

    public void OnClick_SaveData()
    {
        if (_auth.CurrentUser != null)
        {
            SaveUserData(_auth.CurrentUser);
            logger.Log("Save User Data");
        }
        else
        {
            logger.Log("현재 로그인된 사용자가 없습니다.");
        }
    }

    public void OnClick_LoadData(string userId)
    {
        LoadUserData(userId);
        logger.Log("Load User Data");
    }

    public void OnClick_ResetData(string userId)
    {
        ResetUserData(userId);
        logger.Log("Reset User Data");
    }
}
