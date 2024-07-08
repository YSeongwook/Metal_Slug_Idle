using UnityEngine;
using EnumTypes;
using EventLibrary;

public class FollowerController : HeroController
{
    public HeroController leader;
    public Vector3 formationOffset;

    private IFollowerState currentState;
    public readonly FollowState FollowState = new FollowState();
    public readonly AttackState AttackState = new AttackState();
    public readonly FollowManualState ManualState = new FollowManualState();

    private new void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        InitializeFollower();
        EventManager<HeroEvents>.StartListening(HeroEvents.LeaderAttackStarted, OnLeaderAttackStarted);
        EventManager<HeroEvents>.StartListening(HeroEvents.LeaderAttackStopped, OnLeaderAttackStopped);
        EventManager<HeroEvents>.StartListening(HeroEvents.LeaderDirectionChanged, OnLeaderDirectionChanged);
        EventManager<UIEvents>.StartListening(UIEvents.OnTouchStartJoystick, OnUserControl);
        EventManager<UIEvents>.StartListening(UIEvents.OnTouchEndJoystick, OffUserControl);
    }

    public void InitializeFollower()
    {
        this.IsLeader = false;
        if (leader != null)
        {
            if (formationOffset == Vector3.zero)
            {
                formationOffset = transform.position - leader.transform.position;
            }
            TransitionToState(FollowState);
        }
        else
        {
            Debug.LogError("Leader not assigned to follower.");
        }
    }

    private void OnDestroy()
    {
        EventManager<HeroEvents>.StopListening(HeroEvents.LeaderAttackStarted, OnLeaderAttackStarted);
        EventManager<HeroEvents>.StopListening(HeroEvents.LeaderAttackStopped, OnLeaderAttackStopped);
        EventManager<HeroEvents>.StopListening(HeroEvents.LeaderDirectionChanged, OnLeaderDirectionChanged);
        EventManager<UIEvents>.StopListening(UIEvents.OnTouchStartJoystick, OnUserControl);
        EventManager<UIEvents>.StopListening(UIEvents.OnTouchEndJoystick, OffUserControl);
    }

    // 매 프레임 상태 업데이트
    private void Update()
    {
        if (currentState == null) return;
        currentState.UpdateState();
    }

    // 고정된 시간 간격으로 물리 업데이트
    private void FixedUpdate()
    {
        if (currentState == null) return;
        currentState.PhysicsUpdateState();
    }

    private void OnLeaderAttackStarted()
    {
        EventManager<HeroEvents>.StopListening(HeroEvents.LeaderDirectionChanged, OnLeaderDirectionChanged);
        TransitionToState(AttackState);
    }

    private void OnLeaderAttackStopped()
    {
        TransitionToState(FollowState);
        EventManager<HeroEvents>.StartListening(HeroEvents.LeaderDirectionChanged, OnLeaderDirectionChanged);
    }

    private void OnLeaderDirectionChanged()
    {
        if (currentState != AttackState) // AttackState일 때는 무시
        {
        }
    }

    private void OnUserControl()
    {
        IsUserControlled = true;
        TransitionToState(ManualState);
    }

    public void TransitionToState(IFollowerState state)
    {
        if (currentState == state) return; // 상태 전환 빈도 줄이기
        currentState?.ExitState();
        currentState = state;
        currentState.EnterState(this);
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
    }
}
