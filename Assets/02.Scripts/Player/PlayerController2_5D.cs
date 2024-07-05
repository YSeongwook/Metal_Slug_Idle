using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2_5D : MonoBehaviour
{
    public float moveSpeed = 5f; // 기본 이동 속도
    public float zSpeedMultiplier = 1.78f; // 16:9 비율에 맞춘 세로 이동 속도 배수 (16/9 = 1.78)
    public float flipSpeed = 10f; // 스프라이트 반전 속도
    public Transform hud; // HUD의 Transform을 public으로 선언하여 에디터에서 할당

    private Vector2 _moveInput;
    private Rigidbody _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Camera _mainCamera;
    private BoxCollider _boxCollider; // BoxCollider 참조
    private float _initialY; // 초기 y값을 저장할 변수
    private bool _isFlipped = false; // 스프라이트가 반전되었는지 여부
    private Vector3 _boxColliderCenter; // BoxCollider의 중심점 저장 변수
    private Vector3 _hudPosition; // HUD의 위치 저장 변수
    
    private static readonly int Speed = Animator.StringToHash("Speed"); // 이동속도 애니메이터 파라미터
    private static readonly int Attack = Animator.StringToHash("Attack"); // 공격 애니메이터 파라미터

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _moveInput = Vector2.zero;
        _rb = GetComponent<Rigidbody>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _mainCamera = Camera.main; // 메인 카메라 참조
        _initialY = transform.position.y; // 초기 y값 저장
        _boxCollider = GetComponent<BoxCollider>(); // BoxCollider 참조
        _boxColliderCenter = _boxCollider.center; // 초기 BoxCollider 중심점 저장
        if (hud == null)
        {
            hud = GetComponentInChildren<Transform>(); // HUD의 Transform 참조, 에디터에서 할당되지 않은 경우
        }
        _hudPosition = hud.localPosition; // 초기 HUD 위치 저장
    }
    private void FixedUpdate()
    {
        // 이동 방향 설정
        Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y * zSpeedMultiplier);

        // 현재 위치에서 y값을 초기값으로 고정
        Vector3 newPosition = _rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime);
        newPosition.y = _initialY;

        // 캐릭터 이동
        _rb.MovePosition(newPosition);

        // 애니메이터에 속도 전달
        _animator.SetFloat(Speed, moveDirection.magnitude);

        // 캐릭터 회전 처리 및 스프라이트 반전
        if (moveDirection != Vector3.zero)
        {
            HandleSpriteFlip(_moveInput.x);
        }
    }
    
    // Unity 이벤트로 호출되는 함수
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    // Unity 이벤트로 호출되는 함수
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _animator.SetTrigger(Attack);
        }
    }

    // 스프라이트 좌우 반전 처리 및 BoxCollider, HUD 위치 조정
    private void HandleSpriteFlip(float moveHorizontal)
    {
        if (moveHorizontal == 0) return;

        bool shouldFlip = moveHorizontal < 0;

        if (shouldFlip == _isFlipped) return;

        _isFlipped = shouldFlip;
        _spriteRenderer.flipX = shouldFlip;

        // BoxCollider 위치 반전
        _boxColliderCenter.x *= -1;
        _boxCollider.center = _boxColliderCenter;

        // HUD 위치 반전
        _hudPosition.x *= -1;
        hud.localPosition = _hudPosition;
    }
}
