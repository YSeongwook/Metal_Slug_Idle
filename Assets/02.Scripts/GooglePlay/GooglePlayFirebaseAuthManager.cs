using EnumTypes;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using GooglePlayGames;
using EventLibrary;
using GooglePlayGames.BasicApi;

public class GooglePlayFirebaseAuthManager : MonoBehaviour
{
    public FirebaseDataManager firebaseDataManager;
    private FirebaseAuth _auth;

    private Logger logger;

    // 초기화 시 실행
    private void Start()
    {
        logger = Logger.Instance;
        ConfigureGooglePlayGames();

        // Firebase 인증 및 데이터베이스 초기화
        try
        {
            _auth = FirebaseAuth.DefaultInstance;

            if (_auth == null)
            {
                logger.Log("인증 인스턴스가 null 입니다.");
            }

            logger.Log("FirebaseAuth 초기화 완료.");
        }
        catch (System.Exception e)
        {
            logger.LogError("Firebase 초기화 실패: " + e.Message);
        }

        // 자동 로그인 시도
        TryAutoLogin();
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
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickSignInGoogle);  // 구글 로그인 이벤트 발생
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                logger.Log("Google Play Games 로그인 성공");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        SignInWithFirebase(code);
                    }
                    else
                    {
                        logger.LogError("서버 인증 코드가 비어 있습니다.");
                        if (isAutoLogin)
                        {
                            HandleAutoLoginFailure();
                        }
                    }
                });
            }
            else
            {
                logger.LogError("Google Play Games 로그인 실패");
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
        logger.LogError("자동 로그인에 실패했습니다.");
    }

    // Firebase를 통한 로그인
    private void SignInWithFirebase(string authCode)
    {
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                logger.LogError("Firebase 자격 증명 로그인이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                logger.LogError("Firebase 자격 증명 로그인 중 오류 발생: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            logger.Log($"{newUser.DisplayName ?? "이름 없음"}로 로그인 했습니다.");
            firebaseDataManager.SaveUserData(newUser);
        });
    }
    
    // 로그인 버튼 클릭 시 실행
    public void OnClick_SignIn()
    {
        SignInWithGooglePlay(false);
        logger.Log("Google Play Services를 통한 로그인 시도");
    }
}
