using EnumTypes;
using EventLibrary;

public class ShoUIEventHandlers : Singleton<ShoUIEventHandlers>, IUIEventHandlers
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
        EventManager<GachaEvents>.StartListening(GachaEvents.GachaSingle, _uiManager.OnClickSummonButton);
        EventManager<GachaEvents>.StartListening(GachaEvents.GachaTen, _uiManager.OnClickSummonButton);
        EventManager<GachaEvents>.StartListening(GachaEvents.GachaThirty, _uiManager.OnClickSummonButton);
    }

    public void RemoveEvents()
    {
        EventManager<GachaEvents>.StopListening(GachaEvents.GachaSingle, _uiManager.OnClickSummonButton);
        EventManager<GachaEvents>.StopListening(GachaEvents.GachaTen, _uiManager.OnClickSummonButton);
        EventManager<GachaEvents>.StopListening(GachaEvents.GachaThirty, _uiManager.OnClickSummonButton);
    }

    public void AddButtonClickEvents()
    {
        
    }

    public void RemoveButtonClickEvents()
    {
        
    }
}
