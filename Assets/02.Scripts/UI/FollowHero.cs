using System.Collections;
using UnityEngine;

public class FollowHero : MonoBehaviour
{
    public Transform hero; // 부모 3D 캐릭터

    private Camera mainCamera;
    private Vector3 lastCharacterPosition;

    void Start()
    {
        mainCamera = Camera.main;
        lastCharacterPosition = hero.position;
        StartCoroutine(UpdateUIPositionCoroutine());
    }

    IEnumerator UpdateUIPositionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // 0.1초마다 체크

            if (hero.position != lastCharacterPosition)
            {
                UpdateUIPosition();
                lastCharacterPosition = hero.position;
            }
        }
    }

    void UpdateUIPosition()
    {
        // 부모 캐릭터의 월드 좌표를 화면 좌표로 변환합니다.
        Vector3 screenPos = mainCamera.WorldToScreenPoint(hero.position);

        // 화면 좌표를 Canvas의 좌표로 변환합니다.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            screenPos,
            mainCamera,
            out Vector2 hudLocalPos
        );

        // HUD Canvas의 위치를 업데이트합니다.
        (transform as RectTransform).anchoredPosition = hudLocalPos;
    }
}