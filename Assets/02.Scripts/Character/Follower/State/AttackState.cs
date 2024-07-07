using UnityEngine;

public class AttackState : IFollowerState
{
    private FollowerController _follower;
    private Transform targetEnemy;
    private const float CheckInterval = 0.5f; // 리더 이동 감지 간격
    private float _lastCheckTime;
    private const float MaxDistanceFromLeader = 10f; // 리더로부터의 최대 거리

    public void EnterState(FollowerController follower)
    {
        _follower = follower;
        targetEnemy = _follower.CurrentTarget;
        _lastCheckTime = Time.time;
        _follower.Animator.SetBool(_follower.IsAttacking, true);
        Debug.Log("Enter FollowerAttackState");
    }

    public void UpdateState()
    {
        if (_follower.IsUserControlled)
        {
            _follower.TransitionToState(_follower.ManualState);
            return;
        }

        if (Time.time - _lastCheckTime >= CheckInterval)
        {
            _lastCheckTime = Time.time;
            CheckLeaderDistance();
        }

        if (targetEnemy == null || Vector3.Distance(_follower.transform.position, targetEnemy.position) > _follower.heroStats.attackRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            Attack();
        }
    }

    public void PhysicsUpdateState() { }

    public void ExitState()
    {
        targetEnemy = null;
        _follower.Animator.SetBool(_follower.IsAttacking, false);
    }

    private void CheckLeaderDistance()
    {
        float distanceToLeader = Vector3.Distance(_follower.transform.position, _follower.leader.transform.position);
        if (distanceToLeader > MaxDistanceFromLeader)
        {
            _follower.TransitionToState(_follower.FollowState);
        }
    }

    private void MoveTowardsTarget()
    {
        if (targetEnemy != null)
        {
            Vector3 direction = (targetEnemy.position - _follower.transform.position).normalized;
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.z) * (_follower.heroStats.moveSpeed * Time.deltaTime);
            _follower.Rb.MovePosition(_follower.Rb.position + moveDirection);
            _follower.Animator.SetFloat(_follower.SpeedParameter, moveDirection.magnitude);
        }
    }

    private void Attack()
    {
        _follower.Animator.SetTrigger(_follower.AttackParameter);

        if (targetEnemy != null)
        {
            // targetEnemy.GetComponent<Enemy>().TakeDamage(_follower.heroStats.attack);
        }
    }
}
