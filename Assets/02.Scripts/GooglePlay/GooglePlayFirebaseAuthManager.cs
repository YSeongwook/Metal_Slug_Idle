using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using GooglePlayGames;

public class GooglePlayFirebaseAuthManager : MonoBehaviour
{
    public Text LogMessage;
    
    private string message;
    private FirebaseAuth auth;

    private void Start()
    {
        ConfigureGooglePlayGames();
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log(auth);
    }

    private void FixedUpdate()
    {
        LogMessage.text = message;
    }

    public void OnClick_SignIn()
    {
        SignInWithGooglePlay();
        Debug.Log("Google Play Services를 통한 로그인 시도");
    }

    private void ConfigureGooglePlayGames()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    private void SignInWithGooglePlay()
    {
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("Google Play Games 로그인 성공");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        Debug.Log("Server auth code: " + code);
                        SignInWithFirebase(code);
                    }
                    else
                    {
                        Debug.LogError("서버 인증 코드가 비어 있습니다.");
                        message = "서버 인증 코드가 비어 있습니다.";
                    }
                });
            }
            else
            {
                Debug.LogError("Google Play Games 로그인 실패");
                message = "Google Play Games 로그인에 실패했습니다.";
            }
        });
    }

    private void SignInWithFirebase(string authCode)
    {
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                message = "Firebase 로그인 취소";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                message = "Firebase 로그인 오류: " + task.Exception;
                return;
            }

            FirebaseUser newUser = task.Result;
            message = newUser.Email + " 로 로그인 했습니다.";
            Debug.LogFormat("Firebase 사용자 로그인: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }
}
