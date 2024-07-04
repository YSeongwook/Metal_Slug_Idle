using EnumTypes;
using EventLibrary;

public class UIEventHandlers : Singleton<UIEventHandlers>
{
    protected UIManager _uiManager;

    protected override void Awake()
    {
        base.Awake();
        _uiManager = UIManager.Instance;
        AddEvents(); // 이벤트 리스너 등록
        AddButtonClickEvents(); // 버튼 클릭 이벤트 등록
    }

    protected virtual void OnDestroy()
    {
        RemoveEvents(); // 이벤트 리스너 제거
        RemoveButtonClickEvents(); // 버튼 클릭 이벤트 제거
    }

    // 이벤트를 등록하는 메서드
    protected virtual void AddEvents()
    {

    }

    // 이벤트 리스너를 제거하는 메서드
    protected virtual void RemoveEvents()
    {
        
    }

    // 버튼 클릭 이벤트 리스너를 등록하는 메서드
    protected virtual void AddButtonClickEvents()
    {

    }

    // 버튼 클릭 이벤트 리스너를 제거하는 메서드
    protected virtual void RemoveButtonClickEvents()
    {

    }
}
