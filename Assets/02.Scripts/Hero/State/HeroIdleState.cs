using UnityEngine;

public class HeroIdleState : IHeroState
{
    private HeroController _hero;
    private const float CheckInterval = 0.5f; // 적 탐지 간격 (0.5초)
    private float _lastCheckTime;

    public void EnterState(HeroController hero)
    {
        _hero = hero;
        _lastCheckTime = Time.time; // 마지막 체크 시간 초기화
        _hero.Animator.SetFloat(_hero.SpeedParameter, 0); // 속도 0으로 설정, Idle 애니메이션 재생
        
        Debug.Log("Enter IdleState");
    }

    public void UpdateState()
    {
        if (_hero.IsUserControlled) 
        {
            _hero.TransitionToState(_hero.ManualState); // 수동 조작 시 수동 모드로 변경
            return;
        }

        if (Time.time - _lastCheckTime >= CheckInterval)
        {
            _lastCheckTime = Time.time;
            FindClosestEnemy(); // 적 탐지
        }
    }

    public void PhysicsUpdateState()
    {
        // 물리 업데이트 로직 필요 시 작성
    }

    public void ExitState()
    {
        // 상태를 나갈 때 수행할 작업 작성
    }

    // 가장 가까운 적을 탐지하여 상태를 전환
    private void FindClosestEnemy()
    {
        float detectionRange = _hero.IsAutoMode ? 30f : _hero.heroStats.attackRange * 5; // 합리적인 탐지 범위 설정
        Collider[] enemies = Physics.OverlapSphere(_hero.transform.position, detectionRange, _hero.EnemyLayer); // 범위 내 모든 적 탐지
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies) // 모든 적 순회
        {
            float distanceToEnemy = Vector3.Distance(_hero.transform.position, enemy.transform.position); // 적과의 거리 계산
            
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy; // 가장 가까운 적 갱신
                closestEnemy = enemy.transform; // 가장 가까운 적의 트랜스폼 저장
            }
        }

        if (closestEnemy == null) return; // 가장 가까운 적이 없으면 리턴
        
        if (closestDistance <= _hero.heroStats.attackRange) // 가장 가까운 적이 공격 범위 내에 있으면
        {
            _hero.TransitionToState(_hero.AttackState); // 공격 상태로 전환
        }
        else if (_hero.IsAutoMode) // 오토 모드인 경우
        {
            _hero.TransitionToState(_hero.MoveState); // 이동 상태로 전환
        }
    }
}
