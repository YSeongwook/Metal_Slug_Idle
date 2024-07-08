using UnityEngine;
using EnumTypes;
using EventLibrary;

public class FollowerController : HeroController
{
    public HeroController leader;
    public SpriteRenderer leaderSpriteRenderer;
    public Vector3 formationOffset;
    public float followDistance = 5f;

    private IFollowerState currentState;
    public readonly FollowState FollowState = new FollowState();
    public readonly AttackState AttackState = new AttackState();
    public readonly FollowManualState ManualState = new FollowManualState();

    private new void Awake()
    {
        Initialize();
        InitializeFollower();
        EventManager<HeroEvents>.StartListening(HeroEvents.LeaderAttackStarted, OnLeaderAttackStarted);
        EventManager<HeroEvents>.StartListening(HeroEvents.LeaderAttackStopped, OnLeaderAttackStopped);
        EventManager<HeroEvents>.StartListening(HeroEvents.LeaderDirectionChanged, OnLeaderDirectionChanged);
        EventManager<UIEvents>.StartListening(UIEvents.OnTouchStartJoystick, OnUserControl);
        EventManager<UIEvents>.StartListening(UIEvents.OnTouchEndJoystick, OffUserControl);
    }
    
    private void InitializeFollower()
    {
        this.IsLeader = false;
        leaderSpriteRenderer = leader.SpriteRenderer;
        if (formationOffset == Vector3.zero)
        {
            formationOffset = transform.position - leader.transform.position;
        }
        TransitionToState(FollowState);
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
        currentState.UpdateState();
        // 방향은 여기서 관리하는게 좋을 듯, 이동 하는 방향을 바라보게?
    }

    // 고정된 시간 간격으로 물리 업데이트
    private void FixedUpdate()
    {
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
            SpriteRenderer.flipX = !SpriteRenderer.flipX;
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
