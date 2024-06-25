using EnumTypes;
using EventLibrary;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
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

    [BoxGroup("Main UI/Buttons", centerLabel: true)]
    public Button chatButton;

    [TabGroup("Intro UI")] public GameObject introUI;

    [TabGroup("Loading UI")] public GameObject loadingUI;

    [TabGroup("Chat UI")] public GameObject chatUI;
    
    [TabGroup("Joystick UI")] public GameObject joystickUI;

    [TabGroup("Data Panel")] public GameObject dataPanel;

    [TabGroup("Player Information")] public TextMeshProUGUI displayNameText; // displayName을 표시할 TMP Text
    [TabGroup("Player Information")] public TextMeshProUGUI levelText; // level을 표시할 TMP Text
    [TabGroup("Player Information")] public TextMeshProUGUI itemsText; // items를 표시할 TMP Text

    [TabGroup("Log Scroll View")] public GameObject logScrollView;
    [TabGroup("Log Scroll View")] public Button logButton;

    private bool _isActiveChatUI;

    #endregion

    // 싱글톤 초기화 및 버튼 클릭 이벤트 등록
    protected override void Awake()
    {
        base.Awake();
        AddInputFieldEvents(); // InputField 이벤트 등록
    }

    private void Start()
    {
        // 인트로 UI와 로그 버튼을 제외하고 자식 오브젝트 모두 비활성화
        foreach (Transform child in transform)
        {
            if (child.gameObject != introUI && child.gameObject != logButton.gameObject)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    // 이벤트 리스너 제거
    private void OnDestroy()
    {
        RemoveInputFieldEvents(); // InputField 이벤트 제거
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

    // 채팅 UI를 토글하는 메서드
    public void ToggleChatUI()
    {
        chatUI.SetActive(!chatUI.activeSelf);
    }

    // 로그 UI를 토글하는 메서드
    public void ToggleLog()
    {
        logScrollView.SetActive(!logScrollView.activeSelf);
    }

    // 로그 창을 닫는 메서드
    public void ExitLog()
    {
        logScrollView.SetActive(false);
        signInUI.SetActive(false);
        dataPanel.SetActive(true);
    }

    // 수동 Google Sign-In 이벤트를 트리거하는 메서드
    public void ManualGoogleSignIn()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickManualGPGSSignIn);
    }

    // 이메일 로그인 UI를 활성화하는 메서드
    public void EnableEmailSignInUI()
    {
        signInButtons.SetActive(false);
        emailSignIn.SetActive(true);
    }

    // 로딩 UI를 표시하는 메서드
    public void ShowLoadingUI()
    {
        loadingUI.SetActive(true);
    }

    // 로딩 UI를 숨기는 메서드
    public void HideLoadingUI()
    {
        loadingUI.SetActive(false);
        introUI.SetActive(false);
        mainUI.SetActive(true);
        joystickUI.SetActive(true);
    }

    // 로그인 UI를 활성화하는 메서드
    public void EnableSignInUI()
    {
        signInUI.SetActive(true);
    }

    // 로그인 UI를 비활성화하는 메서드
    public void DisableSignInUI()
    {
        signInUI.SetActive(false);
    }
    
    // 이메일 로그인 UI를 비활성화하는 메서드
    public void DisableEmailSignInUI()
    {
        signInUI.SetActive(false);
    }

    // 인트로 UI를 비활성화하는 메서드
    public void DisableIntroUI()
    {
        introUI.SetActive(false);
    }

    // 유저 데이터가 로드될 때 호출되는 메서드
    public void OnUserDataLoaded(User user)
    {
        displayNameText.text = $"Name: {user.displayName}";
        levelText.text = $"Level: {user.level}";
        itemsText.text = $"Items: {user.items}";
    }

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
        EventSystem.current.SetSelectedGameObject(null);

        if (TouchScreenKeyboard.visible)
        {
            TouchScreenKeyboard.hideInput = true;
        }
    }
}
