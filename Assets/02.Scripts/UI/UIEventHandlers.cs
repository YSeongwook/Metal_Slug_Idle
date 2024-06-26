using EnumTypes;
using EventLibrary;

public class UIEventHandlers : Singleton<UIEventHandlers>
{
    private UIManager _uiManager;

    private void Awake()
    {
        _uiManager = UIManager.Instance;
        AddEvents(); // 이벤트 리스너 등록
        AddButtonClickEvents(); // 버튼 클릭 이벤트 등록
    }

    private void OnDestroy()
    {
        RemoveEvents(); // 이벤트 리스너 제거
        RemoveButtonClickEvents(); // 버튼 클릭 이벤트 제거
    }

    // 이벤트를 등록하는 메서드
    private void AddEvents()
    {
        EventManager<UIEvents>.StartListening(UIEvents.OnClickSignInGoogle, _uiManager.DisableSignInUI);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickStart, _uiManager.DisableIntroUI);
        EventManager<UIEvents>.StartListening(UIEvents.StartLoading, _uiManager.ShowLoadingUI);
        EventManager<UIEvents>.StartListening(UIEvents.EndLoading, _uiManager.HideLoadingUI);
    }

    // 이벤트 리스너를 제거하는 메서드
    private void RemoveEvents()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickSignInGoogle, _uiManager.DisableSignInUI);
        EventManager<UIEvents>.StopListening(UIEvents.OnClickStart, _uiManager.DisableIntroUI);
        EventManager<UIEvents>.StopListening(UIEvents.StartLoading, _uiManager.ShowLoadingUI);
        EventManager<UIEvents>.StopListening(UIEvents.EndLoading, _uiManager.HideLoadingUI);
    }

    // 버튼 클릭 이벤트 리스너를 등록하는 메서드
    private void AddButtonClickEvents()
    {
        _uiManager.chatButton.onClick.AddListener(OnClickChatButton);
        _uiManager.logButton.onClick.AddListener(OnClickLogButton);
        _uiManager.signInGoogle.onClick.AddListener(OnClickManualGoogleSignIn);
        _uiManager.signInEmail.onClick.AddListener(OnClickEmailSignInButton);
    }

    // 버튼 클릭 이벤트 리스너를 제거하는 메서드
    private void RemoveButtonClickEvents()
    {
        _uiManager.chatButton.onClick.RemoveListener(OnClickChatButton);
        _uiManager.logButton.onClick.RemoveListener(OnClickLogButton);
        _uiManager.signInGoogle.onClick.RemoveListener(OnClickManualGoogleSignIn);
        _uiManager.signInEmail.onClick.RemoveListener(OnClickEmailSignInButton);
    }

    // 채팅 버튼 클릭 이벤트 핸들러
    public void OnClickChatButton()
    {
        _uiManager.ToggleChatUI();
    }

    // 로그 버튼 클릭 이벤트 핸들러
    public void OnClickLogButton()
    {
        _uiManager.ToggleLog();
    }

    // 로그 창 닫기 버튼 클릭 이벤트 핸들러
    public void OnClickExitLog()
    {
        _uiManager.ExitLog();
    }

    // 수동 Google Sign-In 버튼 클릭 이벤트 핸들러
    public void OnClickManualGoogleSignIn()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickManualGPGSSignIn);
    }

    // 이메일 로그인 버튼 클릭 이벤트 핸들러
    public void OnClickEmailSignInButton()
    {
        _uiManager.EnableEmailSignInUI();
    }

    // 이메일 로그인 이벤트 트리거 메서드
    public void OnClickEmailSignIn()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickEmailSignIn);
    }

    // 이메일 입력 필드를 반환하는 메서드
    public string GetEmail()
    {
        return _uiManager.inputFieldID.text;
    }

    // 비밀번호 입력 필드를 반환하는 메서드
    public string GetPassword()
    {
        return _uiManager.inputFieldPW.text;
    }

    // 이메일 로그인 UI를 활성화하는 메서드
    public void EnableEmailSignInUI()
    {
        _uiManager.EnableEmailSignInUI();
    }

    // 이메일 로그인 UI를 비활성화하는 메서드
    public void DisableEmailSignInUI()
    {
        _uiManager.DisableEmailSignInUI();
    }

    // 로그인 UI를 활성화하는 메서드
    public void EnableSignInUI()
    {
        _uiManager.EnableSignInUI();
    }

    // 로그인 UI를 비활성화하는 메서드
    public void DisableSignInUI()
    {
        _uiManager.DisableSignInUI();
    }
    
    // MainHeroButton이 눌렸을 때 발생하는 메서드
    public void OnClickMainHeroButton()
    {
        // _uiManager.
    }
}
