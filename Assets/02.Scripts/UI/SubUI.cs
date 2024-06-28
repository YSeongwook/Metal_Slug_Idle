using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

// SubUI 클래스는 버튼의 스프라이트를 변경하는 기본 기능을 제공합니다.
public class SubUI : MonoBehaviour
{
    // 버튼 UI 요소
    [TabGroup("MainButton")] public Button button;
    // 비활성화된 상태의 스프라이트
    [TabGroup("MainButton")] public Sprite passiveSprite;
    // 활성화된 상태의 스프라이트
    [TabGroup("MainButton")] public Sprite activeSprite;

    // 버튼의 이미지 컴포넌트
    protected Image _buttonImage;

    // 스크립트가 Awake될 때 호출됩니다.
    protected virtual void Awake()
    {
        // 버튼의 이미지 컴포넌트를 가져옵니다.
        _buttonImage = button.GetComponent<Image>();
    }

    // 오브젝트가 활성화될 때 호출됩니다.
    protected virtual void OnEnable()
    {
        // 버튼과 활성화 스프라이트가 null이 아닌 경우
        if (button != null && activeSprite != null)
        {
            // 버튼의 스프라이트를 활성화 스프라이트로 변경합니다.
            _buttonImage.sprite = activeSprite;
        }
    }

    // 오브젝트가 비활성화될 때 호출됩니다.
    protected virtual void OnDisable()
    {
        // 버튼과 비활성화 스프라이트가 null이 아닌 경우
        if (button != null && passiveSprite != null)
        {
            // 버튼의 스프라이트를 비활성화 스프라이트로 변경합니다.
            _buttonImage.sprite = passiveSprite;
        }
    }
}