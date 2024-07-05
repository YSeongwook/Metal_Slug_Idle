public interface IUIEventHandlers
{
    // 이벤트를 등록하는 메서드
    void AddEvents();

    // 이벤트 리스너를 제거하는 메서드
    void RemoveEvents();

    // 버튼 클릭 이벤트 리스너를 등록하는 메서드
    void AddButtonClickEvents();

    // 버튼 클릭 이벤트 리스너를 제거하는 메서드
    void RemoveButtonClickEvents();
}
