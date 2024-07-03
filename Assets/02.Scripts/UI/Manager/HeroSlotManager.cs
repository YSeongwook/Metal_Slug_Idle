using UnityEngine;
using System.Collections.Generic;

public class HeroSlotManager : MonoBehaviour
{
    public List<HeroSlot> heroSlots; // HeroSlot 오브젝트 리스트
    public Transform[] characterTransforms; // 캐릭터 Transform 배열

    private void Start()
    {
        // 각 HeroSlot의 초기 슬롯 인덱스를 설정하고 캐릭터 Transform 배열을 할당합니다.
        for (int i = 0; i < heroSlots.Count; i++)
        {
            heroSlots[i].slotIndex = i;
            heroSlots[i].characterTransforms = characterTransforms;
            heroSlots[i].UpdateCharacterPositions();
        }
    }
}