using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using EventLibrary;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("Sign In UI")]
    public GameObject signInUI;
    public GameObject logScrollView;
    
    [Header("Data Panel")]
    public GameObject dataPanel;
    
    [Header("Player Information")]
    public TextMeshProUGUI displayNameText; // displayName을 표시할 TMP Text
    public TextMeshProUGUI levelText; // level을 표시할 TMP Text
    public TextMeshProUGUI itemsText; // items를 표시할 TMP Text

    protected override void Awake()
    {
        base.Awake();
        EventManager<UIEvents>.StartListening(UIEvents.OnClickSignInGoogle, DisableSignInUI);
        EventManager<DataEvents>.StartListening<User>(DataEvents.OnUserDataLoad, OnUserDataLoaded); // 데이터 로드 이벤트 리스너 추가
    }

    private void DisableSignInUI()
    {
        signInUI.SetActive(false);
        logScrollView.SetActive(true);
    }

    public void OnClick_ExitLog()
    {
        logScrollView.SetActive(false);
        signInUI.SetActive(false);
        dataPanel.SetActive(true);
    }

    private void OnUserDataLoaded(User user)
    {
        displayNameText.text = $"Name: {user.displayName}";
        levelText.text = $"Level: {user.level.ToString()}";
        itemsText.text = $"Items: {user.items}";
    }
}