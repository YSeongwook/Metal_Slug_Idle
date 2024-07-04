public class MainUIEventHandlers : Singleton<MainUIEventHandlers>, IUIEventHandlers
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
        
    }

    public void RemoveEvents()
    {
        
    }

    public void AddButtonClickEvents()
    {
        _uiManager.chatButton.onClick.AddListener(OnClickChatButton);
        _uiManager.mainCloseButton.onClick.AddListener(OnClickMainCloseButton);
    }

    public void RemoveButtonClickEvents()
    {
        _uiManager.chatButton.onClick.RemoveListener(OnClickChatButton);
        _uiManager.mainCloseButton.onClick.RemoveListener(OnClickMainCloseButton);
    }
    
    // 메인 닫기 버튼 클릭 이벤트 핸들러
    private void OnClickMainCloseButton()
    {
        _uiManager.CloseAllUIs();
    }

    // 채팅 버튼 클릭 이벤트 핸들러
    private void OnClickChatButton()
    {
        _uiManager.ToggleChatUI();
    }
}
