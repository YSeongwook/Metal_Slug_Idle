using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RawImage heroRenderTexture; // Render Texture를 표시할 RawImage
    public HeroSlotData heroSlotData; // 영웅 데이터
    private Transform originalParent; // 드래그 시작 시 원래 부모 트랜스폼 저장
    private CanvasGroup canvasGroup; // 드래그 중 상호작용을 비활성화하기 위한 CanvasGroup

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    // 드래그 시작 시 호출되는 메서드
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root); // 최상위로 이동하여 다른 UI 요소 위에 표시
        transform.SetAsLastSibling(); // 드래그 중에 가장 위로 배치
        canvasGroup.alpha = 0.6f; // 드래그 중 투명도 변경
        canvasGroup.blocksRaycasts = false; // 드래그 중 상호작용 비활성화
    }

    // 드래그 중 호출되는 메서드
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // 마우스 위치로 이동
    }

    // 드래그 종료 시 호출되는 메서드
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent); // 원래 부모로 복귀
        transform.localPosition = Vector3.zero; // 위치 초기화
        canvasGroup.alpha = 1f; // 투명도 복원
        canvasGroup.blocksRaycasts = true; // 상호작용 재활성화

        // 드롭된 위치가 슬롯일 경우 교환
        if (eventData.pointerEnter != null)
        {
            HeroSlot targetSlot = eventData.pointerEnter.GetComponentInParent<HeroSlot>();
            if (targetSlot != null && targetSlot != this)
            {
                SwapHeroes(targetSlot); // 영웅 교환
            }
        }
    }

    // 슬롯 간 영웅 데이터를 교환하는 메서드
    private void SwapHeroes(HeroSlot targetSlot)
    {
        HeroSlotData tempHero = targetSlot.heroSlotData;
        targetSlot.SetHero(this.heroSlotData);
        this.SetHero(tempHero);

        Debug.Log("영웅 위치 변경 완료"); // 디버그 메시지 출력
    }

    // 영웅 데이터를 설정하는 메서드
    public void SetHero(HeroSlotData newHero)
    {
        heroSlotData = newHero;
        heroRenderTexture.texture = newHero.heroRenderTexture; // Render Texture 설정
    }
}
