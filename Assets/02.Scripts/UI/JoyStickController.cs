using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class JoyStickController : MonoBehaviour
{
    public GameObject joystick; // 조이스틱 오브젝트 (Joystick)
    public Image joystickBackground; // 조이스틱 배경 이미지
    public Image joystickStick; // 조이스틱 스틱 이미지
    public HeroController heroController; // HeroController 참조
    private Vector2 joystickInitialPosition; // 조이스틱 초기 위치
    private OnScreenStick onScreenStick;

    private void Start()
    {
        // 조이스틱 초기 위치 저장
        joystickInitialPosition = joystick.GetComponent<RectTransform>().anchoredPosition;

        // 초기 알파값을 0으로 설정하여 조이스틱을 보이지 않게 함
        SetJoystickAlpha(0);

        // OnScreenStick 컴포넌트를 찾아 저장
        onScreenStick = joystickStick.GetComponent<OnScreenStick>();

        // EventTrigger 컴포넌트 설정
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        // 터치 시작 이벤트 핸들러
        EventTrigger.Entry touchStartEntry = new EventTrigger.Entry();
        touchStartEntry.eventID = EventTriggerType.PointerDown;
        touchStartEntry.callback.AddListener((eventData) => { OnTouchStart((PointerEventData)eventData); });
        trigger.triggers.Add(touchStartEntry);

        // 터치 이동 이벤트 핸들러
        EventTrigger.Entry touchMoveEntry = new EventTrigger.Entry();
        touchMoveEntry.eventID = EventTriggerType.Drag;
        touchMoveEntry.callback.AddListener((eventData) => { OnTouchMove((PointerEventData)eventData); });
        trigger.triggers.Add(touchMoveEntry);

        // 터치 종료 이벤트 핸들러
        EventTrigger.Entry touchEndEntry = new EventTrigger.Entry();
        touchEndEntry.eventID = EventTriggerType.PointerUp;
        touchEndEntry.callback.AddListener((eventData) => { OnTouchEnd((PointerEventData)eventData); });
        trigger.triggers.Add(touchEndEntry);
    }

    private void OnTouchStart(PointerEventData eventData)
    {
        // 터치 시작 시 자동 모드 비활성화
        // heroController.IsUserControlled = true;
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnTouchStartJoystick);
        
        // 터치 시작 시 조이스틱 위치 변경 및 알파값 255로 설정
        joystick.GetComponent<RectTransform>().position = eventData.position;
        SetJoystickAlpha(1);

        // OnScreenStick의 터치 시작 메서드 호출
        onScreenStick.OnPointerDown(eventData);
    }

    private void OnTouchMove(PointerEventData eventData)
    {
        // OnScreenStick의 터치 이동 메서드 호출
        onScreenStick.OnDrag(eventData);
    }

    private void OnTouchEnd(PointerEventData eventData)
    {
        // 터치 종료 시 자동 모드 활성화
        // heroController.IsUserControlled = false;
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnTouchEndJoystick);
        
        // 터치 종료 시 조이스틱을 초기 위치로 되돌리고 알파값 0으로 설정
        joystick.GetComponent<RectTransform>().anchoredPosition = joystickInitialPosition;
        SetJoystickAlpha(0);

        // OnScreenStick의 터치 종료 메서드 호출
        onScreenStick.OnPointerUp(eventData);

        // 터치 종료 후에도 자동 이동 모드를 유지하려면 주석 처리
        // heroController.SetAutoMove(true);
    }

    private void SetJoystickAlpha(float alpha)
    {
        // 조이스틱 배경과 스틱의 알파값 설정
        Color bgColor = joystickBackground.color;
        bgColor.a = alpha;
        joystickBackground.color = bgColor;

        Color stickColor = joystickStick.color;
        stickColor.a = alpha;
        joystickStick.color = stickColor;
    }
}
