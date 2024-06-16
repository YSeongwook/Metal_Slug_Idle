using EnumTypes;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using EventLibrary;

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
        EventManager<FirebaseEvents>.StartListening<FirebaseUser>(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);

        logger = Logger.Instance;
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 해제
        EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseInitialized, OnFirebaseInitialized);
        EventManager<FirebaseEvents>.StopListening<FirebaseUser>(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
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

    private void OnFirebaseSignIn(FirebaseUser user)
    {
        SaveUserData(user);
    }
    
    private void OnUserLoggedIn(FirebaseUser user)
    {
        currentUser = user;
    }

    public FirebaseUser GetCurrentUser()
    {
        return currentUser;
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
                JsonUtilityManager.SaveToJson(serverData, "UserData.json");
                logger.Log("서버 데이터로 로컬 데이터를 업데이트했습니다.");
            }
            else if (localData.lastUpdated > serverData.lastUpdated)
            {
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
