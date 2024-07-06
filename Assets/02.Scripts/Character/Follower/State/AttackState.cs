using UnityEngine;

public class AttackState : IFollowerState
{
    private FollowerController _follower;
    private Transform currentTargetEnemy;

    public void EnterState(FollowerController follower)
    {
        currentTargetEnemy = FindClosestEnemy(follower);
        
        DebugLogger.Log("Enter FollowerAttackState");
    }

    public void UpdateState()
    {
        if (currentTargetEnemy == null || Vector3.Distance(_follower.transform.position, currentTargetEnemy.position) > _follower.heroStats.attackRange)
        {
            _follower.TransitionToState(_follower.FollowState);
            return;
        }

        // 공격 로직
        _follower.Animator.SetTrigger(_follower.AttackParameter);
        // currentTargetEnemy.GetComponent<Enemy>().TakeDamage(follower.heroStats.attack);

        /*
        if (currentTargetEnemy.GetComponent<Enemy>().IsDead())
        {
            _follower.TransitionToState(_follower.FollowState);
        }
        */
    }

    public void PhysicsUpdateState()
    {
        throw new System.NotImplementedException();
    }

    public void ExitState()
    {
        currentTargetEnemy = null;
    }

    private Transform FindClosestEnemy(FollowerController follower)
    {
        Collider[] enemies = Physics.OverlapSphere(follower.transform.position, follower.heroStats.attackRange, follower.EnemyLayer);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(follower.transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }
}