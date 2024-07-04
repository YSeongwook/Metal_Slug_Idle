using UnityEngine;

public class HeroUIEventHandlers : UIEventHandlers
{
    protected override void AddEvents()
    {
        
    }

    protected override void RemoveEvents()
    {
        
    }

    protected override void AddButtonClickEvents()
    {
        _uiManager.showOnlyOwnedButton.onClick.AddListener(OnClickShowOnlyOwnedButton);
        _uiManager.activeTypeButton.onClick.AddListener(OnClickActiveTypeButton);
        _uiManager.activeRankButton.onClick.AddListener(OnClickActiveGradeButton);
        _uiManager.heroTabInActiveButton.onClick.AddListener(OnClickHeroTabInActiveButton);
        _uiManager.formationTabInActiveButton.onClick.AddListener(OnClickFormationTabInActiveButton);
        _uiManager.sortAttackButton.onClick.AddListener(OnClickSortAttackButton);
    }

    protected override void RemoveButtonClickEvents()
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
    
    private void OnClickHeroTabInActiveButton()
    {
        _uiManager.DisableFormationTab();
    }
    
    private void OnClickFormationTabInActiveButton()
    {
        _uiManager.EnableFormationTab();
    }

    private void OnClickSortAttackButton()
    {
        _uiManager.SortListByAttack();
    }
}
