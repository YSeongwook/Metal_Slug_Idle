using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseAuthManager : MonoBehaviour
{
    public Text SignUpAuthID;
    public Text SignUpAuthPW;
    public Text SignInAuthID;
    public Text SignInAuthPW;
    public Text LogMessage;
    
    private string message;
    private FirebaseAuth auth;

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        
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
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    message = email + " 로 회원가입 하셨습니다.";
                }
                else
                {
                    // 이메일로는 성공, Google로는 실패, 인증서가 문제?, 아직 등록을 안해서 그런듯
                    message = "회원가입에 실패하셨습니다.";
                }
            }
        );
    }

    private void SignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    message = email + " 로 로그인 하셨습니다.";
                }
                else
                {
                    message = "로그인에 실패하셨습니다.";
                }
            }
        );
    }
}