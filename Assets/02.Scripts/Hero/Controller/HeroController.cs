using UnityEngine;
using UnityEngine.InputSystem;

/**
 * @brief 캐릭터의 기본 동작을 정의하는 클래스입니다.
 */
public class HeroController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float zSpeedMultiplier = 1.777f;
    public Transform hud;

    protected Vector2 _moveInput;
    public Rigidbody Rb { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Animator Animator { get; private set; }
    public Camera MainCamera { get; private set; }
    public BoxCollider BoxCollider { get; private set; }
    public float InitialY { get; private set; }
    public bool IsFlipped { get; protected set; }
    public Vector3 BoxColliderCenter { get; protected set; }
    public Vector3 HudPosition { get; protected set; }

    private IHeroState _currentState;
    public IHeroState CurrentState => _currentState;

    public readonly HeroIdleState IdleState = new HeroIdleState();
    public readonly HeroMoveState MoveState = new HeroMoveState();
    public readonly HeroAttackState AttackState = new HeroAttackState();

    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int AttackParameter = Animator.StringToHash("Attack");

    public HeroStatsManager HeroStatsManager { get; private set; }
    public LayerMask EnemyLayer { get; private set; }
    protected bool _isMovingToTarget;
    public bool IsUserControlled { get; protected set; }
    public bool AutoMove { get; private set; }

    public bool IsMoving => _moveInput != Vector2.zero || _isMovingToTarget;

    protected virtual void Awake()
    {
        Initialize();
        TransitionToState(IdleState);
    }

    protected void Initialize()
    {
        _moveInput = Vector2.zero;
        Rb = GetComponent<Rigidbody>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Animator = GetComponentInChildren<Animator>();
        MainCamera = Camera.main;
        InitialY = transform.position.y;
        BoxCollider = GetComponent<BoxCollider>();
        BoxColliderCenter = BoxCollider.center;
        if (hud == null)
        {
            hud = GetComponentInChildren<Transform>();
        }

        HudPosition = hud.localPosition;
        HeroStatsManager = GetComponent<HeroStatsManager>();
        EnemyLayer = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        _currentState.UpdateState();
    }

    protected virtual void FixedUpdate()
    {
        if (_moveInput != Vector2.zero)
        {
            Move();
        }
        else
        {
            _currentState.UpdateState();
        }
    }

    /**
     * @brief 상태 전환 메서드
     */
    public void TransitionToState(IHeroState state)
    {
        _currentState?.ExitState();
        _currentState = state;
        _currentState.EnterState(this);
    }

    /**
     * @brief 이동 처리 메서드
     */
    protected void Move()
    {
        Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y * zSpeedMultiplier);
        Vector3 newPosition = Rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime);
        newPosition.y = InitialY;
        Rb.MovePosition(newPosition);
        Animator.SetFloat(Speed, moveDirection.magnitude);

        if (moveDirection != Vector3.zero)
        {
            HandleSpriteFlip(_moveInput.x);
        }
    }

    /**
     * @brief 타겟으로 이동 메서드
     */
    public void MoveToTarget(Vector3 targetPosition)
    {
        _isMovingToTarget = true;
        TransitionToState(MoveState);
        MoveState.SetTarget(targetPosition);
    }

    /**
     * @brief 이동 중지 메서드
     */
    public void StopMoving()
    {
        _moveInput = Vector2.zero;
        _isMovingToTarget = false;
        Animator.SetFloat(Speed, 0);
        TransitionToState(IdleState);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        IsUserControlled = _moveInput != Vector2.zero;
    }

    // 스프라이트 플립 처리 메서드
    public void HandleSpriteFlip(float moveHorizontal)
    {
        if (moveHorizontal == 0) return;

        bool shouldFlip = moveHorizontal < 0;

        if (shouldFlip == IsFlipped) return;

        IsFlipped = shouldFlip;
        SpriteRenderer.flipX = shouldFlip;

        // BoxCollider 위치 반전
        Vector3 boxColliderCenter = BoxCollider.center;
        boxColliderCenter.x *= -1;
        BoxCollider.center = boxColliderCenter;

        // HUD 위치 반전
        Vector3 hudPosition = hud.localPosition;
        hudPosition.x *= -1;
        hud.localPosition = hudPosition;
    }

    public void SetAutoMove(bool autoMove)
    {
        AutoMove = autoMove;
        if (autoMove)
        {
            TransitionToState(IdleState);
        }
    }
}
