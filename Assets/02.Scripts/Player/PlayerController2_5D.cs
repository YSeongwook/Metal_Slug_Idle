using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2_5D : MonoBehaviour
{
    public float moveSpeed = 5f; // 기본 이동 속도
    public float zSpeedMultiplier = 1.78f; // 16:9 비율에 맞춘 세로 이동 속도 배수 (16/9 = 1.78)
    public float flipSpeed = 10f; // 스프라이트 반전 속도
    private Vector2 moveInput = Vector2.zero;
    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Camera mainCamera;
    private float initialY; // 초기 y값을 저장할 변수

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main; // 메인 카메라 참조
        initialY = transform.position.y; // 초기 y값 저장
    }

    // Unity 이벤트로 호출되는 함수
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Unity 이벤트로 호출되는 함수
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void FixedUpdate()
    {
        // 카메라의 로컬 공간에 맞춘 이동 방향 설정
        Vector3 moveDirection = mainCamera.transform.rotation * new Vector3(moveInput.x, 0, moveInput.y * zSpeedMultiplier);

        // 현재 위치에서 y값을 초기값으로 고정
        Vector3 newPosition = rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime);
        newPosition.y = initialY;

        // 캐릭터 이동
        rb.MovePosition(newPosition);

        // 애니메이터에 속도 전달
        animator.SetFloat("Speed", moveDirection.magnitude);

        // 캐릭터 회전 처리 및 스프라이트 반전
        if (moveDirection != Vector3.zero)
        {
            HandleSpriteFlip(moveInput.x);
        }
    }

    // 스프라이트 좌우 반전 처리
    void HandleSpriteFlip(float moveHorizontal)
    {
        if (moveHorizontal != 0)
        {
            float targetScaleX = moveHorizontal < 0 ? -1 : 1;
            Vector3 newScale = spriteRenderer.transform.localScale;
            newScale.x = Mathf.Lerp(newScale.x, targetScaleX, Time.fixedDeltaTime * flipSpeed);
            spriteRenderer.transform.localScale = newScale;
        }
    }
}
