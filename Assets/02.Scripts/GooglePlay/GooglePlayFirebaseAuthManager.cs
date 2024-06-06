using Firebase;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;
using GooglePlayGames;

public class GooglePlayFirebaseAuthManager : MonoBehaviour
{
    public Text LogMessagePrefab; // 프리팹으로 사용할 Text 오브젝트
    public Transform LogContent; // Content 오브젝트
    public ScrollRect ScrollRect; // ScrollRect 오브젝트
    
    private FirebaseAuth auth;
    private DatabaseReference databaseRef;

    // 초기화 시 실행
    private void Start()
    {
        ConfigureGooglePlayGames();
        
        // Firebase 인증 및 데이터베이스 초기화
        try
        {
            auth = FirebaseAuth.DefaultInstance;
            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

            if (auth == null)
            {
                Log("인증 인스턴스가 null 입니다.");
                Log("데이터베이스 레퍼런스가 null 입니다.");
            }
            
            Log("FirebaseAuth 초기화 완료.");
        }
        catch (System.Exception e)
        {
            LogError("Firebase 초기화 실패: " + e.Message);
        }
        
        // 자동 로그인 시도
        TryAutoLogin();
    }

    // 로그 메시지 출력
    private void Log(string message)
    {
        AddLogMessage(message);
        Debug.Log(message);
    }

    // 에러 로그 메시지 출력
    private void LogError(string message)
    {
        AddLogMessage(message);
        Debug.LogError(message);
    }
    
    // 로그 메시지 추가
    private void AddLogMessage(string message)
    {
        Text newLogText = Instantiate(LogMessagePrefab, LogContent);
        newLogText.text = message;
    }

    // 로그인 버튼 클릭 시 실행
    public void OnClick_SignIn()
    {
        SignInWithGooglePlay(false);
        Log("Google Play Services를 통한 로그인 시도");
    }

    // Google Play Games 초기화
    private void ConfigureGooglePlayGames()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    
    // 자동 로그인 시도
    private void TryAutoLogin()
    {
        SignInWithGooglePlay(true);
    }

    // Google Play Games를 통한 로그인
    private void SignInWithGooglePlay(bool isAutoLogin)
    {
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                Log("Google Play Games 로그인 성공");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        SignInWithFirebase(code);
                    }
                    else
                    {
                        LogError("서버 인증 코드가 비어 있습니다.");
                        if (isAutoLogin)
                        {
                            HandleAutoLoginFailure();
                        }
                    }
                });
            }
            else
            {
                LogError("Google Play Games 로그인 실패");
                if (isAutoLogin)
                {
                    HandleAutoLoginFailure();
                }
            }
        });
    }
    
    // 자동 로그인 실패 처리
    private void HandleAutoLoginFailure()
    {
        LogError("자동 로그인에 실패했습니다.");
        // 여기서 사용자가 로그인할 수 있도록 로그인 UI를 표시할 수 있습니다.
    }

    // Firebase를 통한 로그인
    private void SignInWithFirebase(string authCode)
    {
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                LogError("Firebase 자격 증명 로그인이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                LogError("Firebase 자격 증명 로그인 중 오류 발생: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Log($"{newUser.DisplayName ?? "이름 없음"}로 로그인 했습니다.");
            SaveUserData(newUser);
        });
    }

    // 유저 데이터를 Firebase Realtime Database에 저장
    private void SaveUserData(FirebaseUser user)
    {
        User userData = new User(user.UserId, user.DisplayName, 1, "None");
        string json = JsonUtility.ToJson(userData);
        databaseRef.Child("users").Child(user.UserId).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                LogError("유저 데이터 저장이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                LogError("유저 데이터 저장 중 오류 발생: " + task.Exception);
                return;
            }

            Log("유저 데이터 저장 성공.");
        });
    }

    // 유저 데이터를 Firebase Realtime Database에서 불러오기
    private void LoadUserData(string userId)
    {
        databaseRef.Child("users").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                LogError("유저 데이터 불러오기가 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                LogError("유저 데이터 불러오기 중 오류 발생: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            User userData = JsonUtility.FromJson<User>(snapshot.GetRawJsonValue());
            Log("유저 데이터 불러오기 성공: " + userData.displayName + ", " + userData.level + ", " + userData.items);
        });
    }

    [System.Serializable]
    public class User
    {
        public string userId;
        public string displayName;
        public int level;
        public string items;

        public User(string userId, string displayName, int level, string items)
        {
            this.userId = userId;
            this.displayName = displayName;
            this.level = level;
            this.items = items;
        }
    }
}
