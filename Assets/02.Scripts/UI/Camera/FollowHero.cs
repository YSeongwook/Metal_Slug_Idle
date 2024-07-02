using UnityEngine;

public class FollowHero : MonoBehaviour
{
    public Transform hero; // 부모 3D 캐릭터
    public RectTransform hpBar; // HP 슬라이더바
    public RectTransform cooltimeBar; // 쿨타임 슬라이더바
    public Vector3 offset; // 체력바와 쿨타임바의 오프셋 (캐릭터 하단에 위치시키기 위한 값)

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        UpdateUIPosition();
    }

    void UpdateUIPosition()
    {
        // 부모 캐릭터의 월드 좌표를 화면 좌표로 변환합니다.
        Vector3 worldPosition = hero.localPosition + offset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        // HP 바와 쿨타임 바의 위치를 업데이트합니다.
        UpdateBarPosition(hpBar, screenPos);
        UpdateBarPosition(cooltimeBar, screenPos);
    }

    void UpdateBarPosition(RectTransform bar, Vector3 screenPos)
    {
        // 화면 좌표를 Canvas의 앵커 좌표로 변환합니다.
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bar.parent as RectTransform,
            screenPos,
            mainCamera,
            out localPos
        );

        // 바의 위치를 업데이트합니다.
        bar.anchoredPosition = localPos;
    }
}