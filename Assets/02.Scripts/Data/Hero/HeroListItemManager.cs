public class HeroListItemManager : Singleton<HeroListItemManager>
{
    private HeroListItem _currentSelectedItem;
    private bool _isDeselecting;

    public void RegisterSelection(HeroListItem selectedItem)
    {
        if (_currentSelectedItem != null && _currentSelectedItem != selectedItem)
        {
            _isDeselecting = true;
            _currentSelectedItem.OnClickHeroListItem(); // 현재 선택된 아이템의 클릭 이벤트를 다시 호출하여 비활성화
            _isDeselecting = false;
        }
        _currentSelectedItem = selectedItem;
    }

    public void ClearCurrentSelection()
    {
        _currentSelectedItem = null;
    }
}