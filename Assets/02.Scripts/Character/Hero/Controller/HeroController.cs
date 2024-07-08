using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float zSpeedMultiplier = 1.777f;
    public Transform hud;
    public HeroStats heroStats;
    
    public bool IsLeader { get; set; }
    public Rigidbody Rb { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Animator Animator { get; private set; }
    public Camera MainCamera { get; private set; }
    public BoxCollider BoxCollider { get; private set; }
    public float InitialY { get; private set; }
    public bool IsFlipped { get; set; }
    public Vector3 BoxColliderCenter { get; protected set; }
    public Vector3 HudPosition { get; protected set; }
    public LayerMask EnemyLayer { get; private set; }
    public bool IsUserControlled { get; set; }
    public bool IsAutoMode { get; set; }
    public float LastUserInputTime { get; set; }
    public Vector2 MoveInput { get; set; }
    public Transform CurrentTarget { get; set; }

    private IHeroState _currentState;
    private Collider[] _cachedEnemies;
    private float _lastCacheTime;
    private const float CacheInterval = 1.0f;

    public readonly HeroIdleState IdleState = new HeroIdleState();
    public readonly HeroMoveState MoveState = new HeroMoveState();
    public readonly HeroAttackState AttackState = new HeroAttackState();
    public readonly HeroManualState ManualState = new HeroManualState();

    public readonly int SpeedParameter = Animator.StringToHash("Speed");
    public readonly int AttackParameter = Animator.StringToHash("Attack");
    public readonly int IsAttacking = Animator.StringToHash("IsAttacking");

    protected HeroStatsManager _heroStatsManager;

    // 초기화 작업 수행
    protected virtual void Awake()
    {
        Initialize();
        TransitionToState(IdleState);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickAutoButton, ToggleAutoMode);
        EventManager<UIEvents>.StartListening(UIEvents.OnTouchStartJoystick, OnUserControl);
        EventManager<UIEvents>.StartListening(UIEvents.OnTouchEndJoystick, OffUserControl);
    }

    // 오브젝트 파괴 시 호출
    protected virtual void OnDestroy()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickAutoButton, ToggleAutoMode);
        EventManager<UIEvents>.StopListening(UIEvents.OnTouchStartJoystick, OnUserControl);
        EventManager<UIEvents>.StopListening(UIEvents.OnTouchEndJoystick, OffUserControl);
    }

    // 컴포넌트 초기화
    protected void Initialize()
    {
        _heroStatsManager = GetComponent<HeroStatsManager>();
        _heroStatsManager.Initialize();
        MoveInput = Vector2.zero;
        Rb = GetComponent<Rigidbody>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Animator = GetComponentInChildren<Animator>();
        MainCamera = Camera.main;
        InitialY = transform.position.y;
        BoxCollider = GetComponent<BoxCollider>();
        BoxColliderCenter = BoxCollider.center;
        IsAutoMode = false;
        IsUserControlled = false;
        if (hud == null)
        {
            hud = GetComponentInChildren<Transform>();
        }

        HudPosition = hud.localPosition;
        EnemyLayer = LayerMask.GetMask("Enemy");

        if (_heroStatsManager != null)
        {
            heroStats = _heroStatsManager.GetHeroStats();
        }
    }

    // 매 프레임 상태 업데이트
    protected void Update()
    {
        _currentState.UpdateState();
    }

    // 고정된 시간 간격으로 물리 업데이트
    protected void FixedUpdate()
    {
        _currentState.PhysicsUpdateState();
    }

    // 상태 전환
    public void TransitionToState(IHeroState state)
    {
        if (_currentState == state) return; // 상태 전환 빈도 줄이기
        _currentState?.ExitState();
        _currentState = state;
        _currentState.EnterState(this);
    }
    
    // 이동 입력 처리
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        LastUserInputTime = Time.time;
    }

    // 이동 중지
    public void StopMoving()
    {
        MoveInput = Vector2.zero;
        Animator.SetFloat(SpeedParameter, 0);
        TransitionToState(IdleState);
    }
    
    // 자동 모드 전환
    private void ToggleAutoMode()
    {
        IsAutoMode = !IsAutoMode;
    }

    private void OnUserControl()
    {
        IsUserControlled = true;
        TransitionToState(ManualState);
    }

    protected void OffUserControl()
    {
        IsUserControlled = false;
    }

    // HeroStatsManager 가져오기
    public HeroStatsManager GetHeroStatsManager()
    {
        return _heroStatsManager;
    }

    // 가장 가까운 적 찾기
    public void FindClosestEnemy()
    {
        if (_cachedEnemies == null || Time.time - _lastCacheTime >= CacheInterval)
        {
            _cachedEnemies = Physics.OverlapSphere(transform.position, 30, EnemyLayer);
            _lastCacheTime = Time.time;
        }

        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in _cachedEnemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        CurrentTarget = closestEnemy;
        
        // 적이 발견되면 스프라이트 반전
        if (CurrentTarget != null)
        {
            float moveHorizontal = CurrentTarget.position.x - transform.position.x;
            HandleSpriteFlip(moveHorizontal);
        }
    }

    // 타겟과의 거리 계산
    public float GetDistanceToTarget(Transform target)
    {
        return Vector3.Distance(transform.position, target.position);
    }
    
    // 캐릭터 방향 전환
    public void HandleSpriteFlip(float moveHorizontal)
    {
        if (moveHorizontal == 0) return;

        bool shouldFlip = moveHorizontal < 0;

        if (shouldFlip == IsFlipped) return;

        IsFlipped = shouldFlip;
        SpriteRenderer.flipX = shouldFlip;

        Vector3 boxColliderCenter = BoxCollider.center;
        boxColliderCenter.x *= -1;
        BoxCollider.center = boxColliderCenter;

        Vector3 hudPosition = hud.localPosition;
        hudPosition.x *= -1;
        hud.localPosition = hudPosition;
        
        EventManager<HeroEvents>.TriggerEvent(HeroEvents.LeaderDirectionChanged);
    }
}
