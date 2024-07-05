using UnityEngine;

public class FollowHero : MonoBehaviour
{
    public Transform hero; // 부모 3D 캐릭터
    public Canvas hudCanvas; // HUD 캔버스 (World Space)
    public Vector3 offset; // HUD 오브젝트의 오프셋 (캐릭터 하단에 위치시키기 위한 값)

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateHUDPosition();
    }

    private void UpdateHUDPosition()
    {
        // 부모 캐릭터의 월드 좌표를 HUD 오브젝트에 반영합니다.
        Vector3 worldPosition = hero.position + offset;
        hudCanvas.transform.position = worldPosition;
        hudCanvas.transform.rotation = _mainCamera.transform.rotation;
    }
}