using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int slotIndex; // 슬롯 인덱스
    public Transform heroTransform; // 영웅 Transform
    public Transform renderTextureParty; // Render Texture Party Transform
    public FormationManager formationManager;

    private Transform _originalParent; // 드래그 시작 시 원래 부모 트랜스폼 저장
    private CanvasGroup _canvasGroup; // 드래그 중 상호작용을 비활성화하기 위한 CanvasGroup
    private RectTransform _rectTransform; // RectTransform 참조
    private Vector3 _originalWorldPosition; // 드래그 시작 시 원래 월드 위치 저장
    private Vector3 _originalCharacterPosition; // 드래그 시작 시 캐릭터의 원래 월드 위치 저장
    private HeroSlotTracker _heroSlotTracker; // 렌더 텍스쳐 영웅 heroSlotTracker
    private GameObject _leaderIcon; // 리더 카메라 아이콘
    private GameObject _inGameHero; // 렌더 텍스쳐 영웅에 대응하는 인게임 영웅
    private HeroController _inGameHeroController; // 렌더 텍스쳐 영웅에 대응하는 인게임 영웅
    private bool _isChangeLeaderMode; // 리더 변경 모드 판별 변수
    private bool _flipX; // 영웅 스프라이트 렌더러 flip 변수

    private const float SlotXSpacing = 230f; // 슬롯 간의 x축 간격
    private const float SlotYSpacing = 130f; // 슬롯 간의 y축 간격
    private const float CharXSpacing = 1.2f; // 캐릭터 간의 x축 간격
    private const float CharYSpacing = 0.95f; // 캐릭터 간의 z축 간격 (3D 공간에서는 y가 아닌 z)

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (formationManager == null)
        {
            formationManager = FindObjectOfType<FormationManager>();
        }
        
        EventManager<FormationEvents>.StartListening(FormationEvents.OnChangeLeaderMode, EnableChangeLeaderMode);
    }

    private void Start()
    {
        Initialize(); // InitializeSlot이 모두 완료되고 나서 동작하게 수정해야 안전, 레오나가 할당인 안되는 이슈가 있었음
    }

    private void OnDestroy()
    {
        EventManager<FormationEvents>.StopListening(FormationEvents.OnChangeLeaderMode, EnableChangeLeaderMode);
    }

    public void Initialize()
    {
        // 초기 위치 설정 또는 로드
        if (heroTransform == null) return;
        
        UpdateCharacterPosition();
        _leaderIcon = heroTransform.GetChild(0).gameObject; // 리더 아이콘 캔버스 오브젝트
        _heroSlotTracker = heroTransform.GetComponent<HeroSlotTracker>();
        _inGameHero = _heroSlotTracker.hero;
        _inGameHeroController = _inGameHero.GetComponent<HeroController>();
    }

    public void InitializeSlot(Transform heroTransform, Transform renderTextureParty)
    {
        this.heroTransform = heroTransform;
        this.renderTextureParty = renderTextureParty;

        if (heroTransform == null) return;
        
        heroTransform.GetComponent<HeroSlotTracker>().assignedSlotIndex = slotIndex;
        UpdateCharacterPosition();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        SetLeader(); // 리더 변경 모드 중 영웅 슬롯 클릭 시 클릭된 영웅 슬롯의 영웅 리더로 설정
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (heroTransform == null) return; // 캐릭터가 없는 슬롯은 드래그 불가능

        _originalParent = transform.parent;
        _originalWorldPosition = _rectTransform.position; // 월드 좌표로 위치 저장
        _originalCharacterPosition = heroTransform.position; // 캐릭터의 월드 위치 저장

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (heroTransform == null) return; // 캐릭터가 없는 슬롯은 드래그 불가능

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        _rectTransform.localPosition = localPoint;

        // 슬롯의 이동을 보정하여 캐릭터를 따라 이동
        Vector3 slotDelta = _rectTransform.position - _originalWorldPosition;
        Vector3 characterDelta = new Vector3(slotDelta.x / SlotXSpacing * CharXSpacing, 0, slotDelta.y / SlotYSpacing * CharYSpacing);
        heroTransform.position = _originalCharacterPosition + characterDelta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (heroTransform == null) return; // 캐릭터가 없는 슬롯은 드래그 불가능

        transform.SetParent(_originalParent);
        _rectTransform.position = _originalWorldPosition; // 월드 좌표로 위치 설정
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        if (eventData.pointerEnter != null)
        {
            HeroSlot targetSlot = eventData.pointerEnter.GetComponentInParent<HeroSlot>();
            if (targetSlot != null && targetSlot != this)
            {
                SwapSlots(targetSlot);
            }
            else
            {
                ReturnToOriginalPosition();
            }
        }
        else
        {
            ReturnToOriginalPosition();
        }

        // 드랍 시 캐릭터 위치 고정
        FixCharacterPosition();
    }

    private void SwapSlots(HeroSlot targetSlot)
    {
        if (targetSlot.heroTransform == null)
        {
            bool isLeader = _heroSlotTracker.hero.GetComponent<HeroController>().IsLeader;

            _heroSlotTracker.UpdateOffsetBasedOnSlotIndex(slotIndex, targetSlot.slotIndex);
            _heroSlotTracker.assignedSlotIndex = targetSlot.slotIndex;
            
            // 캐릭터가 없는 슬롯과 교환
            targetSlot.heroTransform = this.heroTransform;
            targetSlot.Initialize();
            
            this.heroTransform = null;
            this._heroSlotTracker = null;
            
            targetSlot.UpdateCharacterPosition();
            if (isLeader) formationManager.UpdateFormationOffSets(); // 교환하는 오브젝트가 리더인 경우 오프셋 재설정
        }
        else if (heroTransform != null)
        {
            // 캐릭터가 있는 슬롯과 교환
            int oldSlotIndex = slotIndex;
            int targetOldSlotIndex = targetSlot.slotIndex;

            (targetSlot.slotIndex, this.slotIndex) = (this.slotIndex, targetSlot.slotIndex);

            // 위치 교환 (월드 좌표 사용)
            (targetSlot._rectTransform.position, this._rectTransform.position) = (this._rectTransform.position, targetSlot._rectTransform.position);

            // 캐릭터 위치 교환
            (targetSlot.heroTransform.position, this.heroTransform.position) = (this.heroTransform.position, targetSlot.heroTransform.position);

            // 슬롯 인덱스 갱신
            targetSlot._heroSlotTracker.assignedSlotIndex = targetSlot.slotIndex;
            _heroSlotTracker.assignedSlotIndex = slotIndex;

            // 오프셋 업데이트
            if (_inGameHeroController.IsLeader || targetSlot._inGameHeroController.IsLeader)
            {
                targetSlot._heroSlotTracker.UpdateOffsetBasedOnSlotIndex(targetOldSlotIndex, targetSlot.slotIndex);
                _heroSlotTracker.UpdateOffsetBasedOnSlotIndex(oldSlotIndex, slotIndex);
                formationManager.UpdateFormationOffSets();
            }
            else
            {
                targetSlot._heroSlotTracker.UpdateOffsetBasedOnSlotIndex(targetOldSlotIndex, targetSlot.slotIndex);
                _heroSlotTracker.UpdateOffsetBasedOnSlotIndex(oldSlotIndex, slotIndex);
            }

            // 캐릭터의 위치를 업데이트합니다.
            targetSlot.UpdateCharacterPosition();
            UpdateCharacterPosition();
            
            EventManager<UIEvents>.TriggerEvent(UIEvents.FormationChanged);
        }

        // 드랍 시 캐릭터 위치 고정
        FixCharacterPosition();
        targetSlot.FixCharacterPosition();
    }

    private void FixCharacterPosition()
    {
        if (heroTransform == null) return;

        // 현재 슬롯 인덱스를 기반으로 캐릭터 위치 고정
        float fixedX = 0f;
        float fixedZ = 0f;

        fixedX = (slotIndex % 3) switch
        {
            0 => -1.2f,
            1 => 0f,
            2 => 1.2f,
            _ => fixedX
        };

        fixedZ = (slotIndex / 3) switch
        {
            0 => 15.95f,
            1 => 15f,
            2 => 14.05f,
            _ => fixedZ
        };

        heroTransform.localPosition = new Vector3(fixedX, heroTransform.localPosition.y, fixedZ);
    }

    private void UpdateCharacterPosition()
    {
        if (heroTransform == null || renderTextureParty == null) return;

        // 위치 보정
        Vector3 adjustedPosition = renderTextureParty.TransformPoint(heroTransform.localPosition);
        heroTransform.position = adjustedPosition;
    }

    private void ReturnToOriginalPosition()
    {
        _rectTransform.position = _originalWorldPosition; // 유효한 슬롯이 없으면 원래 위치로 돌아감
        heroTransform.position = _originalCharacterPosition; // 캐릭터도 원래 위치로 돌아감
    }

    private void EnableChangeLeaderMode()
    {
        _isChangeLeaderMode = true;
    }
    
    private void SetLeader()
    {
        if (!_isChangeLeaderMode) return;
        
        if (formationManager != null)
        {
            formationManager.SetLeader(_inGameHero);
        }
        
        _isChangeLeaderMode = false;
        
        EventManager<FormationEvents>.TriggerEvent(FormationEvents.SetLeader);
        
        _leaderIcon.SetActive(true);
    }
}
