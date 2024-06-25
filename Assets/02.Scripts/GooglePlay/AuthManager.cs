using EnumTypes;
using EventLibrary;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;

public class AuthManager : Singleton<AuthManager>
{
    public TextMeshProUGUI noticeText; // 상태 메시지를 표시할 UI 텍스트
    public TextMeshProUGUI signInText; // 로그인 상태를 표시할 UI 텍스트

    private FirebaseAuth _auth; // Firebase 인증 객체
    private FirebaseUser currentUser;
    private bool isSignin; // 로그인 상태를 추적하는 플래그
    private Logger logger;

    protected override void Awake()
    {
        base.Awake();
        
        logger = Logger.Instance;

        EventManager<UIEvents>.StartListening(UIEvents.OnClickManualGPGSSignIn, ManualGoogleSignIn);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickEmailSignIn, SignInWithEmail);
    }

    private void Start()
    {
        InitializeFirebase();

#if !UNITY_EDITOR
        TryAutoSignIn();
#else
        TryAutoSignIn();
#endif
    }

    private void TryAutoSignIn()
    {
        PlayGamesPlatform.Activate(); // Google Play Games 활성화
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication); // 자동 로그인 시도
    }

    private void ProcessAuthentication(SignInStatus status)
    {
        signInText.text = "GOOGLE Sign In";

        if (status == SignInStatus.Success)
        {
            logger.Log("Google Play Games 로그인 성공");

            PlayGamesPlatform.Instance.RequestServerSideAccess(false, code =>
            {
                if (!string.IsNullOrEmpty(code))
                {
                    SignInWithFirebase(code);
                    signInText.text = "Success GOOGLE Sign In";
                }
                else
                {
                    logger.LogError("서버 인증 코드가 비어 있습니다.");
                }
            });
        }
        else
        {
            logger.LogError("Google Play Games 로그인 실패");
            signInText.text = "Fail GOOGLE Sign In";
            signInText.text = "GOOGLE LOGIN";
            UIEventHandlers.Instance.EnableSignInUI();
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string errorMessage = "Firebase Initialize Failed: " + task.Exception?.ToString();
                noticeText.text = errorMessage;
                logger.LogError(errorMessage);
                return;
            }

            noticeText.text = "Firebase Initialize Complete";
            FirebaseApp app = FirebaseApp.DefaultInstance;
            _auth = FirebaseAuth.DefaultInstance;

            EventManager<FirebaseEvents>.TriggerEvent(FirebaseEvents.FirebaseInitialized);
        });
    }

    private void SignInWithFirebase(string authCode)
    {
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                logger.LogError("Firebase 자격 증명 로그인 중 오류 발생: " + task.Exception);
                return;
            }

            currentUser = task.Result;
            logger.Log($"{currentUser.DisplayName ?? "이름 없음"}로 로그인 했습니다.");
            signInText.text = "Firebase Login";
            isSignin = true;

            EventManager<FirebaseEvents>.TriggerEvent(FirebaseEvents.FirebaseSignIn);
        });
    }

    public FirebaseUser GetCurrentUser()
    {
        return currentUser;
    }

    private void ManualGoogleSignIn()
    {
        signInText.text = "로그인 시도 중...";
        UIEventHandlers.Instance.DisableSignInUI();
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }

    private void SignInWithEmail()
    {
        string email = UIEventHandlers.Instance.GetEmail();
        string password = UIEventHandlers.Instance.GetPassword();

        Debug.Log($"이메일 로그인 시도: {email}");

        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                FirebaseException firebaseEx = task.Exception?.InnerException as FirebaseException;
                if (firebaseEx != null && firebaseEx.ErrorCode == 1)
                {
                    RegisterAndSignInWithEmail(email, password);
                }
                else
                {
                    string errorMessage = "이메일 로그인 중 오류 발생";
                    if (firebaseEx != null)
                    {
                        errorMessage += $": {firebaseEx.Message} (Error Code: {firebaseEx.ErrorCode})";
                    }
                    else if (task.Exception != null)
                    {
                        errorMessage += $": {task.Exception.Message}";
                    }

                    logger.LogError(errorMessage);
                    noticeText.text = errorMessage;
                    UIEventHandlers.Instance.EnableEmailSignInUI();
                }
                return;
            }

            currentUser = task.Result.User;
            logger.Log($"{currentUser.DisplayName ?? "이름 없음"}로 이메일 로그인 했습니다.");
            signInText.text = "이메일 로그인 성공";
            isSignin = true;

            EventManager<FirebaseEvents>.TriggerEvent(FirebaseEvents.FirebaseSignIn);
            UIEventHandlers.Instance.DisableEmailSignInUI();
        });
    }

    private void RegisterAndSignInWithEmail(string email, string password)
    {
        Debug.Log($"이메일 회원가입 시도: {email}");

        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string errorMessage = "이메일 회원가입 중 오류 발생: " + task.Exception?.ToString();
                logger.LogError(errorMessage);
                noticeText.text = errorMessage;
                UIEventHandlers.Instance.EnableEmailSignInUI();
                return;
            }

            currentUser = task.Result.User;

            // DisplayName 설정
            UserProfile profile = new UserProfile { DisplayName = "User_" + email.Split('@')[0] };
            currentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(updateTask =>
            {
                if (updateTask.IsCanceled || updateTask.IsFaulted)
                {
                    logger.LogError("DisplayName 설정 중 오류 발생: " + updateTask.Exception?.ToString());
                    noticeText.text = "DisplayName 설정 중 오류 발생: " + updateTask.Exception?.ToString();
                    UIEventHandlers.Instance.EnableEmailSignInUI();
                    return;
                }

                logger.Log($"{currentUser.DisplayName ?? "이름 없음"}로 이메일 회원가입 및 DisplayName 설정 완료");
                signInText.text = "이메일 회원가입 성공";
                
                // 회원가입 후 자동 로그인
                SignInWithEmail();
            });
        });
    }
}
