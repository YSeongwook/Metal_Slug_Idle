using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int slotIndex; // 슬롯 인덱스
    public Transform[] characterTransforms; // 캐릭터의 위치를 변경하기 위한 Transform 배열
    private Transform originalParent; // 드래그 시작 시 원래 부모 트랜스폼 저장
    private CanvasGroup canvasGroup; // 드래그 중 상호작용을 비활성화하기 위한 CanvasGroup
    private RectTransform rectTransform; // RectTransform 참조
    private Vector3 originalWorldPosition; // 드래그 시작 시 원래 월드 위치 저장

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        // 초기 위치 설정 또는 로드
        UpdateCharacterPositions();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalWorldPosition = rectTransform.position; // 월드 좌표로 위치 저장
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, eventData.position, eventData.pressEventCamera, out localPoint);
        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
                rectTransform.position = originalWorldPosition; // 유효한 슬롯이 없으면 원래 위치로 돌아감
            }
        }
        else
        {
            rectTransform.position = originalWorldPosition; // 유효한 슬롯이 없으면 원래 위치로 돌아감
        }
    }

    private void SwapSlots(HeroSlot targetSlot)
    {
        int tempSlotIndex = targetSlot.slotIndex;
        targetSlot.slotIndex = this.slotIndex;
        this.slotIndex = tempSlotIndex;

        // 위치 교환 (월드 좌표 사용)
        Vector3 tempPosition = targetSlot.rectTransform.position;
        targetSlot.rectTransform.position = this.rectTransform.position;
        this.rectTransform.position = tempPosition;

        // 부모 교환
        Transform tempParent = targetSlot.transform.parent;
        targetSlot.transform.SetParent(this.originalParent);
        this.transform.SetParent(tempParent);

        // 캐릭터의 위치를 업데이트합니다.
        targetSlot.UpdateCharacterPositions();
        UpdateCharacterPositions();
    }

    public void UpdateCharacterPositions()
    {
        // 각 슬롯의 인덱스에 따라 캐릭터의 위치를 변경합니다.
        for (int i = 0; i < characterTransforms.Length; i++)
        {
            if (i == slotIndex)
            {
                characterTransforms[i].gameObject.SetActive(true);
                characterTransforms[i].localPosition = new Vector3(0, 0, 0); // 슬롯 위치에 맞게 조정
            }
            else
            {
                characterTransforms[i].gameObject.SetActive(false);
            }
        }
    }
}
