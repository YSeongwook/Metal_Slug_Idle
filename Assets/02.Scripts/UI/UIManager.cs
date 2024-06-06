using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using EventLibrary;

public class UIManager : Singleton<UIManager>
{
    public GameObject signInUI;
    public GameObject logScrollView;

    protected override void Awake()
    {
        base.Awake();
        EventManager<UIEvents>.StartListening(UIEvents.OnClickSignInGoogle, DisableSignInUI);
    }

    private void DisableSignInUI()
    {
        signInUI.SetActive(false); // SignInUI를 비활성화
        logScrollView.SetActive(true);
    }
}
