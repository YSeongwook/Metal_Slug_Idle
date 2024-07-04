using EnumTypes;
using EventLibrary;

public class HeroUIEventHandlers : Singleton<HeroUIEventHandlers>, IUIEventHandlers
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
        _uiManager.showOnlyOwnedButton.onClick.AddListener(OnClickShowOnlyOwnedButton);
        _uiManager.activeTypeButton.onClick.AddListener(OnClickActiveTypeButton);
        _uiManager.activeRankButton.onClick.AddListener(OnClickActiveGradeButton);
        _uiManager.heroTabInActiveButton.onClick.AddListener(OnClickHeroTabInActiveButton);
        _uiManager.formationTabInActiveButton.onClick.AddListener(OnClickFormationTabInActiveButton);
        _uiManager.sortAttackButton.onClick.AddListener(OnClickSortAttackButton);
    }

    public void RemoveButtonClickEvents()
    {
        _uiManager.showOnlyOwnedButton.onClick.RemoveListener(OnClickShowOnlyOwnedButton);
        _uiManager.activeTypeButton.onClick.RemoveListener(OnClickActiveTypeButton);
        _uiManager.activeRankButton.onClick.RemoveListener(OnClickActiveGradeButton);
        _uiManager.heroTabInActiveButton.onClick.RemoveListener(OnClickHeroTabInActiveButton);
        _uiManager.formationTabInActiveButton.onClick.RemoveListener(OnClickFormationTabInActiveButton);
        _uiManager.sortAttackButton.onClick.RemoveListener(OnClickSortAttackButton);
    }
    
    private void OnClickShowOnlyOwnedButton()
    {
        _uiManager.ToggleShowOnlyOwnedButton();
    }

    private void OnClickActiveTypeButton()
    {
        _uiManager.ToggleTypeButtonsPanel();
    }

    private void OnClickActiveGradeButton()
    {
        _uiManager.ToggleGradeButtonsPanel();
    }
    
    private void OnClickSortAttackButton()
    {
        _uiManager.SortListByAttack();
    }
    
    private void OnClickHeroTabInActiveButton()
    {
        _uiManager.DisableFormationTab();
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickHeroTabButton);
    }
    
    private void OnClickFormationTabInActiveButton()
    {
        _uiManager.EnableFormationTab();
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickFormationTabButton);
    }
}
