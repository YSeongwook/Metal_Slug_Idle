using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2_5D : MonoBehaviour
{
    public float moveSpeed = 2f; // 기본 이동 속도
    public float zSpeedMultiplier = 1.777f; // 16:9 비율에 맞춘 세로 이동 속도 배수 (16/9 = 1.78)
    public float flipSpeed = 10f; // 스프라이트 반전 속도
    
    private Vector2 _moveInput;
    private Rigidbody _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Camera _mainCamera;
    private float _initialY; // 초기 y값을 저장할 변수
    private Quaternion _cameraRotation; // 카메라 회전을 캐싱할 변수
    private bool _moveInputChanged; // 이동 입력이 변경되었는지 여부를 저장할 변수
    
    private static readonly int Speed = Animator.StringToHash("Speed"); // 이동 속도 애니메이터 파라미터
    private static readonly int Attack = Animator.StringToHash("Attack"); // 공격 애니메이터 파라미터

    private void Awake()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (_moveInputChanged)
        {
            _moveInputChanged = false;

            // 카메라의 로컬 공간에 맞춘 이동 방향 설정
            Vector3 moveDirection = _cameraRotation * new Vector3(_moveInput.x, 0, _moveInput.y * zSpeedMultiplier);

            // 현재 위치에서 y값을 초기값으로 고정
            Vector3 newPosition = _rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime);
            newPosition.y = _initialY;

            // 캐릭터 이동
            _rb.MovePosition(newPosition);

            // 애니메이터에 속도 전달
            _animator.SetFloat(Speed, moveDirection.magnitude);

            // 캐릭터 회전 처리 및 스프라이트 반전
            HandleSpriteFlip(_moveInput.x);
        }
    }
    
    private void Initialize()
    {
        _moveInput = Vector2.zero;
        _rb = GetComponent<Rigidbody>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _mainCamera = Camera.main; // 메인 카메라 참조
        _initialY = transform.position.y; // 초기 y값 저장
        _cameraRotation = _mainCamera.transform.rotation; // 카메라 회전 캐싱
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 newMoveInput = context.ReadValue<Vector2>();
        if (newMoveInput != _moveInput)
        {
            _moveInput = newMoveInput;
            _moveInputChanged = true; // 이동 입력이 변경되었음을 표시
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _animator.SetTrigger(Attack);
        }
    }

    // 스프라이트 좌우 반전 처리
    private void HandleSpriteFlip(float moveHorizontal)
    {
        if (moveHorizontal == 0) return;
        
        float targetScaleX = moveHorizontal < 0 ? -1 : 1;
        Vector3 newScale = _spriteRenderer.transform.localScale;
        newScale.x = Mathf.Lerp(newScale.x, targetScaleX, Time.fixedDeltaTime * flipSpeed);
        _spriteRenderer.transform.localScale = newScale;
    }
}
