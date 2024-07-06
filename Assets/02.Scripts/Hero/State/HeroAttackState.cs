using UnityEngine;

public class HeroAttackState : IHeroState
{
    private HeroController _hero;
    private Transform _targetEnemy;
    private float _lastAttackTime;
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private const float MaxDistanceFromLeader = 10f; // 리더로부터의 최대 거리

    public void EnterState(HeroController hero)
    {
        _hero = hero;
        _lastAttackTime = Time.time - 1f / _hero.heroStats.attackSpeed; // 첫 공격이 바로 실행되도록 설정
        FindClosestEnemy();
        _hero.Animator.SetBool(IsAttacking, true); // 공격 상태 진입 시 애니메이션 트리거 설정
        Debug.Log("Enter AttackState");
    }

    public void UpdateState()
    {
        if (_hero.IsUserControlled)
        {
            _hero.TransitionToState(_hero.ManualState); // 수동 조작 시 수동 모드로 변경
            return;
        }

        if (_targetEnemy == null || !_targetEnemy.gameObject.activeSelf)
        {
            FindClosestEnemy();
            if (_targetEnemy == null)
            {
                _hero.TransitionToState(_hero.IdleState);
                return;
            }
        }

        CheckDistanceAndAttack();
    }

    public void PhysicsUpdateState()
    {
        // 물리 업데이트 로직이 필요할 경우 여기에 작성합니다.
    }

    public void ExitState()
    {
        _hero.Animator.SetBool(IsAttacking, false); // 공격 상태 종료 시 애니메이션 트리거 해제
        // 상태를 나갈 때 수행할 작업이 있으면 여기에 작성합니다.
    }

    // 가장 가까운 적을 찾습니다.
    private void FindClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(_hero.transform.position, 30f, _hero.EnemyLayer); // 합리적인 탐지 범위 사용
        _targetEnemy = GetClosestEnemy(enemies);
    }

    // 주어진 적 목록에서 가장 가까운 적을 반환합니다.
    private Transform GetClosestEnemy(Collider[] enemies)
    {
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider enemy in enemies)
        {
            float distanceToEnemy = GetFlatDistance(_hero.gameObject, enemy.gameObject);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    // 적과의 거리를 확인하고 공격하거나 이동 상태로 전환합니다.
    private void CheckDistanceAndAttack()
    {
        if (_targetEnemy == null) return;

        float distanceToEnemy = GetFlatDistance(_hero.gameObject, _targetEnemy.gameObject);

        if (_hero is FollowerController follower)
        {
            float distanceToLeader = GetFlatDistance(_hero.gameObject, follower.leader.gameObject);
            if (distanceToLeader > MaxDistanceFromLeader)
            {
                _hero.TransitionToState(_hero.IdleState);
                return;
            }
        }

        if (distanceToEnemy > _hero.heroStats.attackRange)
        {
            _hero.TransitionToState(_hero.MoveState);
        }
        else
        {
            if (Time.time - _lastAttackTime >= 2f / _hero.heroStats.attackSpeed)
            {
                Attack();
                _lastAttackTime = Time.time;
            }
        }

        _hero.HandleSpriteFlip((_targetEnemy.position - _hero.transform.position).x);
    }

    // 적을 공격합니다.
    private void Attack()
    {
        DebugLogger.Log("Attack");
        _hero.Animator.SetTrigger(_hero.AttackParameter);

        if (_targetEnemy != null)
        {
            // 적에게 데미지 입히기
            // _targetEnemy.GetComponent<Enemy>().TakeDamage(_hero.heroStats.attack);
        }
    }

    // 공격 애니메이션이 완료되었는지 확인합니다.
    private bool IsAttackAnimationComplete()
    {
        AnimatorStateInfo stateInfo = _hero.Animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f;
    }

    // 두 게임 오브젝트 간의 평면 거리를 반환합니다.
    private float GetFlatDistance(GameObject obj1, GameObject obj2)
    {
        Vector3 pos1 = obj1.transform.position;
        Vector3 pos2 = obj2.transform.position;
        pos1.y = 0;
        pos2.y = 0;
        return Vector3.Distance(pos1, pos2); // Y 좌표를 0으로 설정한 위치의 거리 반환
    }
}
