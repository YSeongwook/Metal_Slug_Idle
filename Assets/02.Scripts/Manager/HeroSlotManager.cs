using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class HeroSlotManager : MonoBehaviour
{
    public Transform renderTextureParty; // Render Texture Party Transform
    [PropertySpace(5f, 0f)]
    public List<HeroSlot> heroSlots; // HeroSlot 오브젝트 리스트

    private void Start()
    {
        FindAssignedSlotIndex();
    }

    // Render Texture Party의 자식들을 순차적으로 접근하여 HeroSlotTracker로부터 현재 할당된 슬롯 인덱스를 찾음
    private void FindAssignedSlotIndex()
    {
        foreach (Transform heroTransform in renderTextureParty)
        {
            HeroSlotTracker tracker = heroTransform.GetComponent<HeroSlotTracker>();

            if (tracker == null) continue;

            int assignedSlotIndex = tracker.assignedSlotIndex;
            if (assignedSlotIndex >= 0 && assignedSlotIndex < heroSlots.Count)
            {
                heroSlots[assignedSlotIndex].InitializeSlot(heroTransform, renderTextureParty);
            }
        }
    }
}