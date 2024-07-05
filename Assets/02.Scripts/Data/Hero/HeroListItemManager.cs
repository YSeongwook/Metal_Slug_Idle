using EnumTypes;
using EventLibrary;

public class HeroListItemManager : Singleton<HeroListItemManager>
{
    private HeroListItem _currentSelectedItem;
    private bool _isFormationTabActive;

    protected override void Awake()
    {
        base.Awake();
        EventManager<UIEvents>.StartListening(UIEvents.OnClickHeroTabButton, ChangeTab);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickFormationTabButton, ChangeTab);
    }

    private void OnDestroy()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickHeroTabButton, ChangeTab);
        EventManager<UIEvents>.StopListening(UIEvents.OnClickFormationTabButton, ChangeTab);
    }

    public void RegisterSelection(HeroListItem selectedItem)
    {
        // 현재 탭이 편성 탭인 경우에만 활성화
        if (!_isFormationTabActive) return;
        
        if (_currentSelectedItem == selectedItem)
        {
            // 이미 선택된 아이템을 다시 클릭한 경우
            selectedItem.DeactivateSelectUI();
            _currentSelectedItem = null;
        }
        else
        {
            if (_currentSelectedItem != null)
            {
                _currentSelectedItem.DeactivateSelectUI(); // 현재 선택된 아이템의 선택 UI 비활성화
            }

            _currentSelectedItem = selectedItem;
            _currentSelectedItem.ActivateSelectUI(); // 새로 선택된 아이템의 선택 UI 활성화
        }
    }

    private void ChangeTab()
    {
        _isFormationTabActive = !_isFormationTabActive; // 현재 탭 상태를 토글
        ClearCurrentSelection();
    }

    private void ClearCurrentSelection()
    {
        if (_currentSelectedItem == null) return;
        
        _currentSelectedItem.DeactivateSelectUI(); // 현재 선택된 아이템의 선택 UI 비활성화
        _currentSelectedItem = null;
    }
}