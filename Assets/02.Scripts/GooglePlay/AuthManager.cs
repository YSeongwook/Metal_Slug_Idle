using EnumTypes;
using EventLibrary;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public TextMeshProUGUI noticeText; // 상태 메시지를 표시할 UI 텍스트
    public TextMeshProUGUI signInText; // 로그인 상태를 표시할 UI 텍스트

    private FirebaseAuth _auth; // Firebase 인증 객체
    private FirebaseUser currentUser;
    private bool isSignin; // 로그인 상태를 추적하는 플래그
    private Logger logger;

    private void Awake()
    {
        logger = Logger.Instance;
        
        EventManager<UIEvents>.StartListening(UIEvents.OnClickManualGPGSSignIn, ManualGoogleSignIn);
    }

    private void Start()
    {
        InitializeFirebase();
        
        // 에디터가 아닌 경우에만 Google Play Games 로그인 시도
#if !UNITY_EDITOR
        TryAutoSignIn();
#else
        // logger.Log("에디터 모드에서는 Google Play Games 로그인을 생략합니다.");
        TryAutoSignIn();
#endif
    }

    // Google Play Games 초기화 및 자동 로그인 시도
    private void TryAutoSignIn()
    {
        PlayGamesPlatform.Activate(); // Google Play Games 활성화
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication); // 자동 로그인 시도
    }
    
    // Google Play Games 인증 상태를 처리하는 메서드
    private void ProcessAuthentication(SignInStatus status)
    {
        signInText.text = "GOOGLE Sign In";
        
        if (status == SignInStatus.Success)
        {
            logger.Log("Google Play Games 로그인 성공");

            // 서버 측 인증 코드를 요청하여 Firebase에 로그인
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, code =>
            {
                if (!string.IsNullOrEmpty(code))
                {
                    SignInWithFirebase(code); // Firebase 인증 처리
                    EventManager<GoogleEvents>.TriggerEvent(GoogleEvents.GPGSSignIn);    // 구글 로그인 성공 이벤트 발생
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

            // 로그인 버튼 활성화 및 상태 메시지 업데이트
            signInText.text = "GOOGLE LOGIN";
            UIManager.Instance.EnableSignInUI(); //  Sign In UI 활성화
        }
    }
    
    // Firebase 초기화
    private void InitializeFirebase()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                noticeText.text = "Firebase Initialize Failed: " + task.Exception?.ToString();
                return;
            }

            noticeText.text = "Firebase Initialize Complete"; // Firebase 초기화 완료 메시지
            FirebaseApp app = FirebaseApp.DefaultInstance; // Firebase 앱 인스턴스 가져오기
            _auth = FirebaseAuth.DefaultInstance; // Firebase 인증 인스턴스 가져오기

            // Firebase 초기화 완료 이벤트 트리거
            EventManager<FirebaseEvents>.TriggerEvent(FirebaseEvents.FirebaseInitialized);
        });
    }

    // Firebase를 통한 로그인 처리 메서드
    private void SignInWithFirebase(string authCode)
    {
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode); // Firebase 자격 증명 생성
        _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                logger.LogError("Firebase 자격 증명 로그인 중 오류 발생: " + task.Exception);
                return;
            }

            currentUser = task.Result; // 로그인 성공 시 사용자 정보 가져오기
            logger.Log($"{currentUser.DisplayName ?? "이름 없음"}로 로그인 했습니다.");
            signInText.text = "Firebase Login"; // 상태 메시지 업데이트
            isSignin = true; // 로그인 상태 업데이트

            // Firebase 로그인 완료 이벤트 트리거
            EventManager<FirebaseEvents>.TriggerEvent(FirebaseEvents.FirebaseSignIn, currentUser);
        });
    }
    
    public FirebaseUser GetCurrentUser()
    {
        return currentUser;
    }

    // 수동 구글 로그인
    private void ManualGoogleSignIn()
    {
        signInText.text = "로그인 시도 중..."; // 상태 메시지 업데이트
        UIManager.Instance.DisableSignInUI(); // Sign In UI 비활성화
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication); // 수동 로그인 시도
    }
}
