using UnityEngine;
using System.Collections.Generic;

public class HeroSlotManager : MonoBehaviour
{
    public List<HeroSlot> heroSlots; // HeroSlot 오브젝트 리스트
    public Transform renderTextureParty; // Render Texture Party Transform

    private void Start()
    {
        // Render Texture Party의 자식들을 순차적으로 접근하여 HeroSlotTracker로부터 현재 할당된 슬롯 인덱스를 찾음
        foreach (Transform characterTransform in renderTextureParty)
        {
            HeroSlotTracker tracker = characterTransform.GetComponent<HeroSlotTracker>();
            if (tracker != null)
            {
                int assignedSlotIndex = tracker.assignedSlotIndex;
                if (assignedSlotIndex >= 0 && assignedSlotIndex < heroSlots.Count)
                {
                    heroSlots[assignedSlotIndex].InitializeSlot(characterTransform, renderTextureParty);
                }
            }
        }
    }
}