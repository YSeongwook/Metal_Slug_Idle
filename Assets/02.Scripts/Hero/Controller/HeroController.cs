using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float zSpeedMultiplier = 1.777f;
    public float autoMoveDelay = 0.5f;
    public Transform hud;
    public HeroStats heroStats;
    
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
    
    private IHeroState _currentState;

    public readonly HeroIdleState IdleState = new HeroIdleState();
    public readonly HeroMoveState MoveState = new HeroMoveState();
    public readonly HeroAttackState AttackState = new HeroAttackState();
    public readonly HeroManualState ManualState = new HeroManualState();

    public readonly int SpeedParameter = Animator.StringToHash("Speed");
    public readonly int AttackParameter = Animator.StringToHash("Attack");
    
    protected bool _isMovingToTarget;

    private HeroStatsManager _heroStatsManager;

    protected virtual void Awake()
    {
        Initialize();
        TransitionToState(IdleState);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickAutoButton, ToggleAutoMode);
    }

    protected virtual void OnDestroy()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickAutoButton, ToggleAutoMode);
    }

    private void Initialize()
    {
        _heroStatsManager = GetComponent<HeroStatsManager>();
        _heroStatsManager.Initialize(); // HeroStatsManager 초기화
        MoveInput = Vector2.zero;
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
        EnemyLayer = LayerMask.GetMask("Enemy");

        if (_heroStatsManager != null)
        {
            heroStats = _heroStatsManager.GetHeroStats(); // HeroStatsManager에서 heroStats를 가져와 할당
        }
    }

    private void Update()
    {
        _currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        _currentState.PhysicsUpdateState();
    }

    public void TransitionToState(IHeroState state)
    {
        _currentState?.ExitState();
        _currentState = state;
        _currentState.EnterState(this);
    }

    public void StopMoving()
    {
        MoveInput = Vector2.zero;
        _isMovingToTarget = false;
        Animator.SetFloat(SpeedParameter, 0);
        TransitionToState(IdleState);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        IsUserControlled = MoveInput != Vector2.zero;
        LastUserInputTime = Time.time;
    }

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
    }

    private void ToggleAutoMode()
    {
        IsAutoMode = !IsAutoMode;
    }

    public HeroStatsManager GetHeroStatsManager()
    {
        return _heroStatsManager;
    }
}
