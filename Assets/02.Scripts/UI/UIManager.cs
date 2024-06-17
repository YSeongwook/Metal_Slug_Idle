using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Inspector Variables

    [FoldoutGroup("Sign In UI")] [PropertySpace(5f, 10f)]
    public GameObject signInUI;

    [HorizontalGroup("Sign In UI/Horizontal")]
    [VerticalGroup("Sign In UI/Horizontal/Left")]
    [BoxGroup("Sign In UI/Horizontal/Left/Buttons", centerLabel: true)]
    public GameObject signInButtons;

    [VerticalGroup("Sign In UI/Horizontal/Left")] [BoxGroup("Sign In UI/Horizontal/Left/Buttons")]
    public Button signInGoogle;

    [VerticalGroup("Sign In UI/Horizontal/Left")] [BoxGroup("Sign In UI/Horizontal/Left/Buttons")]
    public Button signInEmail;

    [VerticalGroup("Sign In UI/Horizontal/Right")]
    [BoxGroup("Sign In UI/Horizontal/Right/Email Sign In", centerLabel: true)]
    public GameObject emailSignIn;

    [VerticalGroup("Sign In UI/Horizontal/Right")] [BoxGroup("Sign In UI/Horizontal/Right/Email Sign In")]
    public TMP_InputField inputFieldID;

    [VerticalGroup("Sign In UI/Horizontal/Right")] [BoxGroup("Sign In UI/Horizontal/Right/Email Sign In")]
    public TMP_InputField inputFieldPW;
    
    [FoldoutGroup("Main UI")] [PropertySpace(5f, 10f)]
    public GameObject mainUI;
    [BoxGroup("Main UI/Buttons", centerLabel: true)] public Button chatButton;
    
    [TabGroup("Intro UI")] public GameObject introUI;
    
    [TabGroup("Loading UI")] public GameObject loadingUI;
    
    [TabGroup("Chat UI")] public GameObject chatUI;
    
    [TabGroup("Data Panel")] public GameObject dataPanel;

    [TabGroup("Player Information")] public TextMeshProUGUI displayNameText; // displayName을 표시할 TMP Text
    [TabGroup("Player Information")] public TextMeshProUGUI levelText; // level을 표시할 TMP Text
    [TabGroup("Player Information")] public TextMeshProUGUI itemsText; // items를 표시할 TMP Text

    [TabGroup("Log Scroll View")] public GameObject logScrollView;

    private bool _isActiveChatUI;

    #endregion

    // 싱글톤 초기화 및 이벤트 등록
    protected override void Awake()
    {
        base.Awake();
        
        AddEvents();
        AddInputFieldEvents();
        AddButtonClickEvents();
    }

    private void Start()
    {
        // Intro UI를 제외하고 자식 오브젝트 모두 비활성화
        foreach (Transform child in transform)
        {
            if (child.gameObject != introUI && child.gameObject != logScrollView)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    // 이벤트 리스너 제거
    private void OnDestroy()
    {
        RemoveEvents();
        RemoveInputFieldEvents();
        RemoveButtonClickEvents();
    }

    // 이벤트를 등록하는 메서드
    private void AddEvents()
    {
        EventManager<UIEvents>.StartListening(UIEvents.OnClickSignInGoogle, DisableSignInUI);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickStart, DisableIntroUI);
        EventManager<UIEvents>.StartListening(UIEvents.StartLoading, ShowLoadingUI);
        EventManager<UIEvents>.StartListening(UIEvents.EndLoading, HideLoadingUI);
        EventManager<DataEvents>.StartListening<User>(DataEvents.OnUserDataLoad, OnUserDataLoaded);
    }

    // 이벤트 리스너를 제거하는 메서드
    private void RemoveEvents()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickSignInGoogle, DisableSignInUI);
        EventManager<UIEvents>.StopListening(UIEvents.OnClickStart, DisableIntroUI);
        EventManager<UIEvents>.StopListening(UIEvents.StartLoading, ShowLoadingUI);
        EventManager<UIEvents>.StopListening(UIEvents.EndLoading, HideLoadingUI);
        EventManager<DataEvents>.StopListening<User>(DataEvents.OnUserDataLoad, OnUserDataLoaded);
    }

    // InputField의 onEndEdit 이벤트 리스너를 등록하는 메서드
    private void AddInputFieldEvents()
    {
        inputFieldID.onEndEdit.AddListener(OnEndEdit);
        inputFieldPW.onEndEdit.AddListener(OnEndEdit);
    }

    // InputField의 onEndEdit 이벤트 리스너를 제거하는 메서드
    private void RemoveInputFieldEvents()
    {
        inputFieldID.onEndEdit.RemoveListener(OnEndEdit);
        inputFieldPW.onEndEdit.RemoveListener(OnEndEdit);
    }

    private void AddButtonClickEvents()
    {
        chatButton.onClick.AddListener(OnClick_ChatButton);
    }

    private void RemoveButtonClickEvents()
    {
        chatButton.onClick.RemoveListener(OnClick_ChatButton);
    }

    // 로딩 UI를 표시하는 메서드
    private void ShowLoadingUI()
    {
        loadingUI.SetActive(true);
    }

    // 로딩 UI를 숨기는 메서드
    private void HideLoadingUI()
    {
        loadingUI.SetActive(false);
        introUI.SetActive(false);
        mainUI.SetActive(true);
    }

    // 로그인 UI를 활성화하는 메서드
    public void EnableSignInUI()
    {
        signInUI.SetActive(true);
        logScrollView.SetActive(true);
    }

    // 로그인 UI를 비활성화하는 메서드
    public void DisableSignInUI()
    {
        signInUI.SetActive(false);
        logScrollView.SetActive(true);
    }

    // 인트로 UI를 비활성화하는 메서드
    private void DisableIntroUI()
    {
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    // 유저 데이터가 로드될 때 호출되는 메서드
    private void OnUserDataLoaded(User user)
    {
        displayNameText.text = $"Name: {user.displayName}";
        levelText.text = $"Level: {user.level.ToString()}";
        itemsText.text = $"Items: {user.items}";
    }

    // 이메일 로그인 UI를 활성화하는 메서드
    public void EnableEmailSignInUI()
    {
        signInButtons.SetActive(false); // 로그인 버튼 UI 비활성화
        emailSignIn.SetActive(true); // 이메일 로그인 UI 활성화
    }
    
    // 이메일 로그인 UI를 비활성화하는 메서드
    public void DisableEmailSignInUI()
    {
        // Sign In UI 비활성화
        emailSignIn.SetActive(false);
        signInButtons.SetActive(true);
        signInUI.SetActive(false);
        
        logScrollView.SetActive(false);
    }

    #region OnClick

    // 로그 창을 닫는 메서드
    public void OnClick_ExitLog()
    {
        logScrollView.SetActive(false);
        signInUI.SetActive(false);
        dataPanel.SetActive(true);
    }

    // 수동 Google Sign-In 버튼 클릭 시 호출되는 메서드
    public void OnClick_ManualGoogleSignIn()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickManualGPGSSignIn);
    }
    
    // 이메일 로그인 버튼 클릭 시 호출되는 메서드
    public void OnClick_EmailSignInButton()
    {
        EnableEmailSignInUI();
    }

    // 이메일 로그인 이벤트 트리거 메서드
    public void OnClick_EmailSignIn()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickEmailSignIn);
    }
    
    // 채팅 버튼 클릭 메서드
    private void OnClick_ChatButton()
    {
        chatUI.SetActive(!chatUI.activeSelf);
    }

    #endregion
    
    // InputField의 onEndEdit 이벤트 핸들러
    private void OnEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CloseSoftKeyboard();
        }
    }

    // 소프트 키보드를 닫는 메서드
    private void CloseSoftKeyboard()
    {
        // 현재 선택된 게임 오브젝트의 입력을 종료
        EventSystem.current.SetSelectedGameObject(null);

        // TouchScreenKeyboard를 사용하여 소프트 키보드를 닫음
        if (TouchScreenKeyboard.visible)
        {
            TouchScreenKeyboard.hideInput = true;
        }
    }
}
