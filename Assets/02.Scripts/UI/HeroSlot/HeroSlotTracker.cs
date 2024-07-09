using UnityEngine;

public class HeroSlotTracker : MonoBehaviour
{
    public int assignedSlotIndex; // 할당된 슬롯의 인덱스
    public GameObject hero;
    
    // 오브젝트의 pos값 변경될 때 가져와서 follower인 경우 followerController offSet 수정
    // 리더인 경우 리더 위치 변경 후 follower 전부 offSet 재설정해야함
}