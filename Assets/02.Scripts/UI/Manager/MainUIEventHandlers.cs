using Unity.VisualScripting;

public class MainUIEventHandlers : UIEventHandlers
{
    protected override void AddEvents()
    {
        
    }

    protected override void RemoveEvents()
    {
        
    }

    protected override void AddButtonClickEvents()
    {
        _uiManager.chatButton.onClick.AddListener(OnClickChatButton);
        _uiManager.mainCloseButton.onClick.AddListener(OnClickMainCloseButton);
    }

    protected override void RemoveButtonClickEvents()
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
