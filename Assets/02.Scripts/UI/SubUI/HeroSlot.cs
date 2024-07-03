using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int slotIndex; // 슬롯 인덱스
    public Transform characterTransform; // 캐릭터의 Transform
    public Transform renderTextureParty; // Render Texture Party Transform

    private Transform originalParent; // 드래그 시작 시 원래 부모 트랜스폼 저장
    private CanvasGroup canvasGroup; // 드래그 중 상호작용을 비활성화하기 위한 CanvasGroup
    private RectTransform rectTransform; // RectTransform 참조
    private Vector3 originalWorldPosition; // 드래그 시작 시 원래 월드 위치 저장
    private Vector3 originalCharacterPosition; // 드래그 시작 시 캐릭터의 원래 월드 위치 저장

    private Camera mainCamera; // 메인 카메라

    private const float slotXSpacing = 230f; // 슬롯 간의 x축 간격
    private const float slotYSpacing = 130f; // 슬롯 간의 y축 간격
    private const float charXSpacing = 1.1f; // 캐릭터 간의 x축 간격
    private const float charYSpacing = 0.95f; // 캐릭터 간의 z축 간격 (3D 공간에서는 y가 아닌 z)

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainCamera = Camera.main;

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        // 초기 위치 설정 또는 로드
        if (characterTransform != null)
        {
            UpdateCharacterPosition();
        }
    }

    public void InitializeSlot(Transform characterTransform, Transform renderTextureParty)
    {
        this.characterTransform = characterTransform;
        this.renderTextureParty = renderTextureParty;

        if (characterTransform == null) return;
        
        characterTransform.GetComponent<HeroSlotTracker>().assignedSlotIndex = slotIndex;
        UpdateCharacterPosition();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (characterTransform == null) return; // 캐릭터가 없는 슬롯은 드래그 불가능

        originalParent = transform.parent;
        originalWorldPosition = rectTransform.position; // 월드 좌표로 위치 저장
        originalCharacterPosition = characterTransform.position; // 캐릭터의 월드 위치 저장

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (characterTransform == null) return; // 캐릭터가 없는 슬롯은 드래그 불가능

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        rectTransform.localPosition = localPoint;

        // 슬롯의 이동을 보정하여 캐릭터를 따라 이동
        Vector3 slotDelta = rectTransform.position - originalWorldPosition;
        Vector3 characterDelta = new Vector3(slotDelta.x / slotXSpacing * charXSpacing, 0, slotDelta.y / slotYSpacing * charYSpacing);
        characterTransform.position = originalCharacterPosition + characterDelta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (characterTransform == null) return; // 캐릭터가 없는 슬롯은 드래그 불가능

        transform.SetParent(originalParent);
        rectTransform.position = originalWorldPosition; // 월드 좌표로 위치 설정
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

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
        if (targetSlot.characterTransform == null)
        {
            // 캐릭터가 없는 슬롯과 교환
            targetSlot.characterTransform = this.characterTransform;
            this.characterTransform = null;

            targetSlot.characterTransform.GetComponent<HeroSlotTracker>().assignedSlotIndex = targetSlot.slotIndex;

            targetSlot.UpdateCharacterPosition();
            this.UpdateCharacterPosition();
        }
        else if (characterTransform != null)
        {
            // 캐릭터가 있는 슬롯과 교환
            (targetSlot.slotIndex, this.slotIndex) = (this.slotIndex, targetSlot.slotIndex);

            // 위치 교환 (월드 좌표 사용)
            (targetSlot.rectTransform.position, this.rectTransform.position) = (this.rectTransform.position, targetSlot.rectTransform.position);

            // 캐릭터 위치 교환
            (targetSlot.characterTransform.position, this.characterTransform.position) = (this.characterTransform.position, targetSlot.characterTransform.position);

            // 슬롯 인덱스 갱신
            targetSlot.characterTransform.GetComponent<HeroSlotTracker>().assignedSlotIndex = targetSlot.slotIndex;
            characterTransform.GetComponent<HeroSlotTracker>().assignedSlotIndex = slotIndex;

            // 캐릭터의 위치를 업데이트합니다.
            targetSlot.UpdateCharacterPosition();
            UpdateCharacterPosition();
        }

        // 드랍 시 캐릭터 위치 고정
        FixCharacterPosition();
        targetSlot.FixCharacterPosition();
    }

    private void FixCharacterPosition()
    {
        if (characterTransform == null) return;

        // 현재 슬롯 인덱스를 기반으로 캐릭터 위치 고정
        float fixedX = 0f;
        float fixedZ = 0f;

        switch (slotIndex % 3)
        {
            case 0:
                fixedX = -1.2f;
                break;
            case 1:
                fixedX = 0f;
                break;
            case 2:
                fixedX = 1.2f;
                break;
        }

        switch (slotIndex / 3)
        {
            case 0:
                fixedZ = 15.95f;
                break;
            case 1:
                fixedZ = 15f;
                break;
            case 2:
                fixedZ = 14.05f;
                break;
        }

        characterTransform.localPosition = new Vector3(fixedX, characterTransform.localPosition.y, fixedZ);
    }

    public void UpdateCharacterPosition()
    {
        if (characterTransform == null || renderTextureParty == null) return;

        // 위치 보정
        Vector3 adjustedPosition = renderTextureParty.TransformPoint(characterTransform.localPosition);
        characterTransform.position = adjustedPosition;
    }

    private void ReturnToOriginalPosition()
    {
        rectTransform.position = originalWorldPosition; // 유효한 슬롯이 없으면 원래 위치로 돌아감
        characterTransform.position = originalCharacterPosition; // 캐릭터도 원래 위치로 돌아감
    }

    private Vector3 ScreenToWorldPosition(Vector3 screenPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
        return worldPosition;
    }
}
