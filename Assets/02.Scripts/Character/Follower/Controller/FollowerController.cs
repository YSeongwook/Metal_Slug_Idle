using UnityEngine;
using EnumTypes;
using EventLibrary;

public class FollowerController : HeroController
{
    public HeroController leader;
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
    }
    
    private void InitializeFollower()
    {
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
    }
    
    // 매 프레임 상태 업데이트
    private void Update()
    {
        currentState.UpdateState();
    }

    // 고정된 시간 간격으로 물리 업데이트
    private void FixedUpdate()
    {
        currentState.PhysicsUpdateState();
    }

    private void OnLeaderAttackStarted()
    {
        TransitionToState(AttackState);
    }

    private void OnLeaderAttackStopped()
    {
        TransitionToState(FollowState);
    }
    

    public void TransitionToState(IFollowerState state)
    {
        if (currentState == state) return; // 상태 전환 빈도 줄이기
        currentState?.ExitState();
        currentState = state;
        currentState.EnterState(this);
    }
}