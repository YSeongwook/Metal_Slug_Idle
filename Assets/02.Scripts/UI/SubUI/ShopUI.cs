using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : SubUI
{
    // 추가 버튼들
    [FoldoutGroup("SelectButton")] public Button selectHeroButton;
    [FoldoutGroup("SelectButton")] public Button selectPetButton;

    [FoldoutGroup("SummonButton")] public Button summonSingleButton;
    [FoldoutGroup("SummonButton")] public Button summonTenButton;
    [FoldoutGroup("SummonButton")] public Button summonThirtyButton;

    [FoldoutGroup("SummonResult")] public GameObject summonResultPanel;
    [FoldoutGroup("SummonResult")] public Button summonResultTenButton;
    [FoldoutGroup("SummonResult")] public Button summonResultThirtyButton;
    [FoldoutGroup("SummonResult")] public Button summonResultCloseButton;
    [FoldoutGroup("SummonResult")] public Button toggleAutoSummonButton;
    [FoldoutGroup("SummonResult")] public GameObject autoSummonCheck;

    // 버튼의 이미지 컴포넌트들
    private Image _selectHeroImage;
    private Image _selectPetImage;
    
    // 자동 소환
    private bool activeAutoSummon;
    
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
            summonResultCloseButton.onClick.AddListener(OnClickCloseButton);
            toggleAutoSummonButton.onClick.AddListener(OnClickToggleAutoSummonButton);
            
            summonSingleButton.onClick.AddListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.GachaSingle));
            summonTenButton.onClick.AddListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.GachaTen));
            summonThirtyButton.onClick.AddListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.GachaThirty));
            summonResultTenButton.onClick.AddListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.AddGachaTen));
            summonResultThirtyButton.onClick.AddListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.AddGachaThirty));
        }
        else
        {
            selectHeroButton.onClick.RemoveListener(OnSelectHeroButtonClicked);
            selectPetButton.onClick.RemoveListener(OnSelectPetButtonClicked);
            summonResultCloseButton.onClick.RemoveListener(OnClickCloseButton);
            toggleAutoSummonButton.onClick.AddListener(OnClickToggleAutoSummonButton);
            
            summonSingleButton.onClick.RemoveListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.GachaSingle));
            summonTenButton.onClick.RemoveListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.GachaTen));
            summonThirtyButton.onClick.RemoveListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.GachaThirty));
            summonResultTenButton.onClick.RemoveListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.AddGachaTen));
            summonResultThirtyButton.onClick.RemoveListener(() => EventManager<GachaEvents>.TriggerEvent(GachaEvents.AddGachaThirty));
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

    public void OnClickCloseButton()
    {
        summonResultPanel.SetActive(false);
        activeAutoSummon = false;
        
        UIManager.Instance.ToggleMainUnderButtons();
    }

    public void OnClickToggleAutoSummonButton()
    {
        bool isActive = autoSummonCheck.activeSelf; // 현재 체크 표시가 활성화 상태인지
        
        autoSummonCheck.SetActive(!isActive); // 비활성화 => 활성화, 활성화 => 비활성화
        activeAutoSummon = !isActive;
    }

    // TODO: 재화가 다 떨어지기 전까지 계속 소환, UserData에서 Card수와 루비 수를 읽어서 결정
    
    // 이미지의 알파 값을 설정하는 메서드
    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
