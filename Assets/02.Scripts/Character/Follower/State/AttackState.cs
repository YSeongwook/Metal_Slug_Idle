using UnityEngine;

public class AttackState : IFollowerState
{
    private FollowerController _follower;
    private Transform targetEnemy;
    private const float CheckInterval = 0.5f; // 리더 이동 감지 간격
    private float _lastCheckTime;
    private const float MaxDistanceFromLeader = 5f; // 리더로부터의 최대 거리
    
    private float originalMoveSpeed;

    public void EnterState(FollowerController follower)
    {
        _follower = follower;
        targetEnemy = _follower.CurrentTarget;
        _lastCheckTime = Time.time;
        originalMoveSpeed = _follower.heroStats.moveSpeed;

        if (targetEnemy == null)
        {
            _follower.FindClosestEnemy();
        }
        
        DebugLogger.Log("Enter FollowerAttackState");
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
            _follower.Animator.SetBool(_follower.IsAttacking, true);
            Attack();
        }
    }

    public void PhysicsUpdateState() { }

    public void ExitState()
    {
        targetEnemy = null;
        _follower.Animator.SetBool(_follower.IsAttacking, false);
        // 포메이션 위치 복귀
        _follower.heroStats.moveSpeed = originalMoveSpeed;
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
        if (targetEnemy == null) return;
        
        Vector3 direction = (targetEnemy.position - _follower.transform.position).normalized;
        Vector3 moveDirection = direction * (_follower.heroStats.moveSpeed * 10 * Time.deltaTime);
        // Vector3 newPosition = _follower.Rb.position + moveDirection;

        // _follower.Rb.MovePosition(newPosition);
        _follower.Rb.MovePosition(Vector3.MoveTowards(_follower.Rb.position, targetEnemy.position, _follower.heroStats.moveSpeed * 1.5f * Time.deltaTime));
        _follower.Animator.SetFloat(_follower.SpeedParameter, moveDirection.magnitude);
    }

    private void Attack()
    {
        _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
        _follower.Animator.SetTrigger(_follower.AttackParameter);

        if (targetEnemy != null)
        {
            // 적의 TakeDamage 메서드 호출
            /*
            Enemy enemy = targetEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_follower.heroStats.attack);
            }
            */
        }
    }
}
