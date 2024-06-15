using EnumTypes;
using UnityEngine;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    #region Inspector Vriables

    [FoldoutGroup("Sign In UI")] [PropertySpace(5f, 10f)]
    public GameObject signInUI;

    [HorizontalGroup("Sign In UI/Horizontal")]
    [VerticalGroup("Sign In UI/Horizontal/Left")]
    [BoxGroup("Sign In UI/Horizontal/Left/Buttons", centerLabel: true)]
    public GameObject signInButtons;

    [VerticalGroup("Sign In UI/Horizontal/Left")] [BoxGroup("Sign In UI/Horizontal/Left/Buttons")]
    public UnityEngine.UI.Button signInGoogle;

    [VerticalGroup("Sign In UI/Horizontal/Left")] [BoxGroup("Sign In UI/Horizontal/Left/Buttons")]
    public UnityEngine.UI.Button signInEmail;

    [VerticalGroup("Sign In UI/Horizontal/Right")]
    [BoxGroup("Sign In UI/Horizontal/Right/Email Sign In", centerLabel: true)]
    public GameObject emailSignIn;

    [VerticalGroup("Sign In UI/Horizontal/Right")] [BoxGroup("Sign In UI/Horizontal/Right/Email Sign In")]
    public TMP_InputField inputFieldID;

    [VerticalGroup("Sign In UI/Horizontal/Right")] [BoxGroup("Sign In UI/Horizontal/Right/Email Sign In")]
    public TMP_InputField inputFieldPW;

    [TabGroup("Data Panel")] public GameObject dataPanel;

    [TabGroup("Player Information")] public TextMeshProUGUI displayNameText; // displayName을 표시할 TMP Text
    [TabGroup("Player Information")] public TextMeshProUGUI levelText; // level을 표시할 TMP Text
    [TabGroup("Player Information")] public TextMeshProUGUI itemsText; // items를 표시할 TMP Text

    [TabGroup("Log Scroll View")] public GameObject logScrollView;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        EventManager<UIEvents>.StartListening(UIEvents.OnClickSignInGoogle, DisableSignInUI);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickStart, DisableIntroUI);
        EventManager<DataEvents>.StartListening<User>(DataEvents.OnUserDataLoad, OnUserDataLoaded); // 데이터 로드 이벤트 리스너 추가
    }

    public void EnableSignInUI()
    {
        signInUI.SetActive(true);
        logScrollView.SetActive(true);
    }

    public void DisableSignInUI()
    {
        signInUI.SetActive(false);
        logScrollView.SetActive(true);
    }

    private void DisableIntroUI()
    {
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void OnUserDataLoaded(User user)
    {
        displayNameText.text = $"Name: {user.displayName}";
        levelText.text = $"Level: {user.level.ToString()}";
        itemsText.text = $"Items: {user.items}";
    }

    public void EnableEmailSignInUI()
    {
        signInButtons.SetActive(false); // 로그인 버튼 UI 비활성화
        emailSignIn.SetActive(true); // 이메일 로그인 UI 활성화
    }
    
    public void OnClick_ExitLog()
    {
        logScrollView.SetActive(false);
        signInUI.SetActive(false);
        dataPanel.SetActive(true);
    }

    public void OnClick_ManualGoogleSignIn()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickManualGPGSSignIn);
    }
    
    public void OnClick_EmailSignIn()
    {
        EnableEmailSignInUI();
    }
}