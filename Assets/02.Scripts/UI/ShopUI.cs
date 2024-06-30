using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

// ShopUI 클래스는 SubUI의 기능을 확장하여 여러 버튼의 스프라이트를 관리합니다.
public class ShopUI : SubUI
{
    // 추가 버튼들
    [FoldoutGroup("SelectButton")] public Button selectHeroButton;
    [FoldoutGroup("SelectButton")] public Button selectPetButton;

    // 버튼의 이미지 컴포넌트들
    private Image _selectHeroImage;
    private Image _selectPetImage;
    
    protected override void Awake()
    {
        // 부모 클래스의 Awake 메서드를 호출
        base.Awake();
        
        _selectHeroImage = selectHeroButton.GetComponent<Image>();
        _selectPetImage = selectPetButton.GetComponent<Image>();
    }
    
    protected override void OnEnable()
    {
        // 부모 클래스의 OnEnable 메서드를 호출
        base.OnEnable();
        // 버튼 클릭 이벤트에 메서드를 등록
        RegisterButtonListeners(true);
        
        UIManager.Instance.OnShopUIEnable();
    }
    
    protected override void OnDisable()
    {
        // 부모 클래스의 OnDisable 메서드를 호출
        base.OnDisable();
        // 버튼 클릭 이벤트에서 메서드를 제거
        RegisterButtonListeners(false);
        
        UIManager.Instance.OnShopUIDisable();
    }

    // 버튼 클릭 이벤트 등록/해제 메서드
    private void RegisterButtonListeners(bool register)
    {
        if (register)
        {
            selectHeroButton.onClick.AddListener(OnSelectHeroButtonClicked);
            selectPetButton.onClick.AddListener(OnSelectPetButtonClicked);
        }
        else
        {
            selectHeroButton.onClick.RemoveListener(OnSelectHeroButtonClicked);
            selectPetButton.onClick.RemoveListener(OnSelectPetButtonClicked);
        }
    }

    // selectHeroButton 클릭 이벤트 핸들러
    private void OnSelectHeroButtonClicked()
    {
        SetImageAlpha(_selectHeroImage, 1f); // 알파값을 1로 설정 (255)
        SetImageAlpha(_selectPetImage, 0f); // 알파값을 0으로 설정
    }

    // selectPetButton 클릭 이벤트 핸들러
    private void OnSelectPetButtonClicked()
    {
        SetImageAlpha(_selectHeroImage, 0f); // 알파값을 0으로 설정
        SetImageAlpha(_selectPetImage, 1f); // 알파값을 1로 설정 (255)
    }

    // 이미지의 알파 값을 설정하는 메서드
    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
