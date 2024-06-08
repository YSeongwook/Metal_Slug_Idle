using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class AuthManager : MonoBehaviour
{
    public FirebaseDataManager firebaseDataManager; // Firebase 데이터 관리자 참조
    public TextMeshProUGUI noticeText; // 상태 메시지를 표시할 UI 텍스트
    public TextMeshProUGUI signInText; // 로그인 상태를 표시할 UI 텍스트

    private FirebaseAuth _auth; // Firebase 인증 객체
    private bool isSignin = false; // 로그인 상태를 추적하는 플래그

    private void Start()
    {
        TryAutoSignIn();
        InitializeFirebase();
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
        if (status == SignInStatus.Success)
        {
            Log("Google Play Games 로그인 성공");

            // 서버 측 인증 코드를 요청하여 Firebase에 로그인
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, code =>
            {
                if (!string.IsNullOrEmpty(code))
                {
                    SignInWithFirebase(code); // Firebase 인증 처리
                }
                else
                {
                    LogError("서버 인증 코드가 비어 있습니다.");
                }
            });
        }
        else
        {
            LogError("Google Play Games 로그인 실패");

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
                LogError("Firebase 자격 증명 로그인 중 오류 발생: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result; // 로그인 성공 시 사용자 정보 가져오기
            Log($"{newUser.DisplayName ?? "이름 없음"}로 로그인 했습니다.");
            signInText.text = "Firebase Login"; // 상태 메시지 업데이트
            isSignin = true; // 로그인 상태 업데이트
            firebaseDataManager.SaveUserData(newUser); // 사용자 데이터 저장
        });
    }

    // 로그인 버튼 클릭 시 실행되는 메서드
    public void OnClick_SignIn()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication); // 수동 로그인 시도
        UIManager.Instance.DisableSignInUI(); // Sign In UI 비활성화
        signInText.text = "로그인 시도 중..."; // 상태 메시지 업데이트
    }

    // 로그 메시지 출력 메서드
    private void Log(string message)
    {
        firebaseDataManager.Log(message); // Firebase 데이터 관리자에 로그 기록
        Debug.Log(message); // Unity 콘솔에 로그 출력
    }

    // 에러 로그 메시지 출력 메서드
    private void LogError(string message)
    {
        firebaseDataManager.Log(message); // Firebase 데이터 관리자에 에러 로그 기록
        Debug.LogError(message); // Unity 콘솔에 에러 로그 출력
    }
}