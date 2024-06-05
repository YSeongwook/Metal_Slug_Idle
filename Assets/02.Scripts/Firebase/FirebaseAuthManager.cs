using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

public class FirebaseAuthManager : MonoBehaviour
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
        SignIn(SignInAuthID.text, SignInAuthPW.text);
        Debug.Log("SignIn : " + SignInAuthID.text + " " + SignInAuthPW.text);
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
                    // 이메일로는 성공, Google로는 실패, 인증서가 문제?, 아직 등록을 안해서 그런듯
                    message = "회원가입에 실패했습니다.";
                }
            }
        );
    }

    private void SignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(
            task =>
            {
                if (task.IsCanceled) {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    message = email + " 로 로그인 했습니다.";
                }
                else
                {
                    message = "로그인에 실패했습니다.";
                }
            }
        );
    }
}