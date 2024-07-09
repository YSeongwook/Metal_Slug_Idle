using UnityEngine;

public class HeroSlotTracker : MonoBehaviour
{
    public int assignedSlotIndex; // 할당된 슬롯의 인덱스
    public GameObject hero;

    private HeroController _heroController;
    private FollowerController _followerController;

    private void Awake()
    {
        _heroController = hero.GetComponent<HeroController>();
        _followerController = hero.GetComponent<FollowerController>();
    }

    public void UpdateOffsetBasedOnSlotIndex(int oldSlotIndex, int newSlotIndex)
    {
        if (_followerController != null)
        {
            Vector3 newOffset = CalculateOffsetChange(oldSlotIndex, newSlotIndex);
            _followerController.formationOffset += newOffset;
        }

        if (_heroController.IsLeader)
        {
            _followerController.formationOffset = Vector3.zero;
        }
    }

    private Vector3 CalculateOffsetChange(int oldSlotIndex, int newSlotIndex)
    {
        float xOffset = ((newSlotIndex % 3) - (oldSlotIndex % 3)) * 1.0f; // x 방향 보정값
        float zOffset = -((newSlotIndex / 3) - (oldSlotIndex / 3)) * 1.4f; // z 방향 보정값

        return new Vector3(xOffset, 0, zOffset);
    }
}