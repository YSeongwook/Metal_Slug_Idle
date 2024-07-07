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

    private new void Awake()
    {
        Initialize();
        TransitionToState(FollowState);
        EventManager<HeroEvents>.StartListening(HeroEvents.LeaderAttackStarted, OnLeaderAttackStarted);
        EventManager<HeroEvents>.StartListening(HeroEvents.LeaderAttackStopped, OnLeaderAttackStopped);
    }

    private void Start()
    {
        // formationOffset이 초기화되지 않은 경우, 리더와의 현재 위치 차이를 기준으로 설정
        if (formationOffset == Vector3.zero)
        {
            formationOffset = transform.position - leader.transform.position;
        }
    }

    private void OnDestroy()
    {
        EventManager<HeroEvents>.StopListening(HeroEvents.LeaderAttackStarted, OnLeaderAttackStarted);
        EventManager<HeroEvents>.StopListening(HeroEvents.LeaderAttackStopped, OnLeaderAttackStopped);
    }
    
    // 매 프레임 상태 업데이트
    protected void Update()
    {
        currentState.UpdateState();
    }

    // 고정된 시간 간격으로 물리 업데이트
    protected void FixedUpdate()
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

    public void TransitionToState(IFollowerState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState(this);
    }
}