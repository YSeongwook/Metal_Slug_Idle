using EnumTypes;
using EventLibrary;

public class IntroUIEventHandlers : Singleton<IntroUIEventHandlers>, IUIEventHandlers
{
    private UIManager _uiManager;
    
    protected override void Awake()
    {
        base.Awake();
        _uiManager = UIManager.Instance;
        
        AddEvents();
        AddButtonClickEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
        RemoveButtonClickEvents();
    }
    
    public void AddEvents()
    {
        EventManager<UIEvents>.StartListening(UIEvents.OnClickSignInGoogle, _uiManager.DisableSignInUI);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickStart, _uiManager.DisableIntroUI);
        EventManager<UIEvents>.StartListening(UIEvents.StartLoading, _uiManager.ShowLoadingUI);
        EventManager<UIEvents>.StartListening(UIEvents.EndLoading, _uiManager.HideLoadingUI);
    }

    public void RemoveEvents()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickSignInGoogle, _uiManager.DisableSignInUI);
        EventManager<UIEvents>.StopListening(UIEvents.OnClickStart, _uiManager.DisableIntroUI);
        EventManager<UIEvents>.StopListening(UIEvents.StartLoading, _uiManager.ShowLoadingUI);
        EventManager<UIEvents>.StopListening(UIEvents.EndLoading, _uiManager.HideLoadingUI);
    }

    public void AddButtonClickEvents()
    {
        _uiManager.logButton.onClick.AddListener(OnClickLogButton);
        _uiManager.signInGoogle.onClick.AddListener(OnClickManualGoogleSignIn);
        _uiManager.signInEmail.onClick.AddListener(OnClickEmailSignInButton);
    }

    public void RemoveButtonClickEvents()
    {
        _uiManager.logButton.onClick.RemoveListener(OnClickLogButton);
        _uiManager.signInGoogle.onClick.RemoveListener(OnClickManualGoogleSignIn);
        _uiManager.signInEmail.onClick.RemoveListener(OnClickEmailSignInButton);
    }
    
    // 로그 버튼 클릭 이벤트 핸들러
    private void OnClickLogButton()
    {
        _uiManager.ToggleLog();
    }

    // 로그 창 닫기 버튼 클릭 이벤트 핸들러
    public void OnClickExitLog()
    {
        _uiManager.ExitLog();
    }

    // 수동 Google Sign-In 버튼 클릭 이벤트 핸들러
    private void OnClickManualGoogleSignIn()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickManualGPGSSignIn);
    }

    // 이메일 로그인 버튼 클릭 이벤트 핸들러
    private void OnClickEmailSignInButton()
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
}
