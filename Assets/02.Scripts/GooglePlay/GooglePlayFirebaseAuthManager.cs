using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayFirebaseAuthManager : MonoBehaviour
{
    public Text SignUpAuthID;
    public Text SignUpAuthPW;
    public Text SignInAuthID;
    public Text SignInAuthPW;
    public Text LogMessage;
    
    private string message;
    private FirebaseAuth auth;

    private void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .RequestEmail()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        auth = FirebaseAuth.DefaultInstance;
        Debug.Log(auth);
    }

    private void FixedUpdate()
    {
        LogMessage.text = message;
    }

    public void OnClick_SignUp()
    {
        SignUp(SignUpAuthID.text, SignUpAuthPW.text);
        Debug.Log("SignUp : " + SignUpAuthID.text + " " + SignUpAuthPW.text);
    }

    public void OnClick_SignIn()
    {
        SignInWithGooglePlay();
        Debug.Log("Google Play Services를 통한 로그인 시도");
    }

    private void SignUp(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(
            task =>
            {
                if (task.IsCanceled) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    message = email + " 로 회원가입 했습니다.";
                }
                else
                {
                    message = "회원가입에 실패했습니다.";
                }
            }
        );
    }

    private void SignInWithGooglePlay()
    {
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("Google Play Games 로그인 성공");
                string idToken = PlayGamesPlatform.Instance.GetIdToken();
                SignInWithFirebase(idToken);
            }
            else
            {
                Debug.Log("Google Play Games 로그인 실패");
                message = "Google Play Games 로그인에 실패했습니다.";
            }
        });
    }

    private void SignInWithFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            message = newUser.Email + " 로 로그인 했습니다.";
            Debug.LogFormat("Firebase 사용자 로그인: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }
}
