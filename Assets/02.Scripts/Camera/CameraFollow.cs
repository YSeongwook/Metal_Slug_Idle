using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 플레이어를 참조
    public Vector3 offset; // 플레이어와의 거리 차이
    public float smoothSpeed = 1f; // 보간 속도

    private void LateUpdate()
    {
        if (player == null) return;
        Vector3 desiredPosition = player.position + offset;

        // Y축 고정
        desiredPosition.y = transform.position.y;

        // 보간하여 부드럽게 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 목표 위치를 계산
    }
}