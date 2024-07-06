using UnityEngine;

public class HeroAttackState : IHeroState
{
    private HeroController _hero;
    private Transform _targetEnemy;
    private float _lastAttackTime;
    private const float CheckInterval = 0.5f; // 공격 간격
    private float _lastCheckTime;
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private const float MaxDistanceFromLeader = 10f;

    public void EnterState(HeroController hero)
    {
        _hero = hero;
        _lastAttackTime = Time.time - 1f / _hero.heroStats.attackSpeed;
        FindClosestEnemy();
        _hero.Animator.SetBool(IsAttacking, true);
        
        DebugLogger.Log("Enter AttackState");
    }

    public void UpdateState()
    {
        if (_hero.IsUserControlled)
        {
            _hero.TransitionToState(_hero.ManualState);
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

        if (Time.time - _lastCheckTime >= CheckInterval)
        {
            _lastCheckTime = Time.time;
            CheckDistanceAndAttack();
        }
    }

    public void PhysicsUpdateState() { }

    public void ExitState()
    {
        _hero.Animator.SetBool(IsAttacking, false);
    }

    private void FindClosestEnemy()
    {
        _hero.FindClosestEnemy();
        _targetEnemy = _hero.CurrentTarget;
    }

    private void CheckDistanceAndAttack()
    {
        if (_targetEnemy == null) return;

        float distanceToEnemy = _hero.GetDistanceToTarget(_targetEnemy);

        if (_hero is FollowerController follower)
        {
            float distanceToLeader = _hero.GetDistanceToTarget(follower.leader.transform);
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

    private void Attack()
    {
        _hero.Animator.SetTrigger(_hero.AttackParameter);

        if (_targetEnemy != null)
        {
            // _targetEnemy.GetComponent<Enemy>().TakeDamage(_hero.heroStats.attack);
        }
    }
}
