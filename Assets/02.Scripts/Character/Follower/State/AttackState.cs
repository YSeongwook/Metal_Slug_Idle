using UnityEngine;

public class AttackState : IFollowerState
{
    private FollowerController _follower;
    private Transform _targetEnemy;
    private const float CheckInterval = 0.5f; // 리더 이동 감지 간격
    private float _lastCheckTime;
    private const float MaxDistanceFromLeader = 5f; // 리더로부터의 최대 거리
    private float _originalMoveSpeed;
    private Vector3 _direction;
    private Vector3 _moveDirection;

    public void EnterState(FollowerController follower)
    {
        _follower = follower;
        _targetEnemy = _follower.CurrentTarget;
        _lastCheckTime = Time.time;
        _originalMoveSpeed = _follower.heroStats.moveSpeed;

        if (_targetEnemy == null)
        {
            // FindClosestEnemy();
            _targetEnemy = _follower.leader.CurrentTarget;
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

        if (_targetEnemy == null)
        {
            FindClosestEnemy();
        }

        if (Vector3.Distance(_follower.transform.position, _targetEnemy.position) > _follower.heroStats.attackRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            _follower.Animator.SetBool(_follower.IsAttacking, true);
            Attack();
        }

        SpriteFlip();
    }

    public void PhysicsUpdateState() { }

    public void ExitState()
    {
        _targetEnemy = null;
        _follower.Animator.SetBool(_follower.IsAttacking, false);
        // 포메이션 위치 복귀
        _follower.heroStats.moveSpeed = _originalMoveSpeed;
    }
    
    private void FindClosestEnemy()
    {
        _follower.FindClosestEnemy();
        
        if (_follower.CurrentTarget == null) return;
        
        float distanceToTarget = _follower.GetDistanceToTarget(_follower.CurrentTarget);
        if (distanceToTarget <= _follower.heroStats.attackRange)
        {
            _follower.TransitionToState(_follower.AttackState);
        }
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
        if (_targetEnemy == null) return;
        
        _direction = (_targetEnemy.position - _follower.transform.position).normalized;
        _moveDirection = _direction * (_follower.heroStats.moveSpeed * 10 * Time.deltaTime);
        // Vector3 newPosition = _follower.Rb.position + moveDirection;

        // _follower.Rb.MovePosition(newPosition);
        _follower.Rb.MovePosition(Vector3.MoveTowards(_follower.Rb.position, _targetEnemy.position, _follower.heroStats.moveSpeed * 1.5f * Time.deltaTime));
        _follower.Animator.SetFloat(_follower.SpeedParameter, _moveDirection.magnitude);
    }

    private void Attack()
    {
        // 적을 바라보고 있게 설정해야함
        
        _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
        _follower.Animator.SetTrigger(_follower.AttackParameter);

        if (_targetEnemy != null)
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

    private void SpriteFlip()
    {
        if (_targetEnemy != null)
        {
            float directionToEnemy = _targetEnemy.position.x - _follower.transform.position.x;
            _follower.HandleSpriteFlip(directionToEnemy);
        }
        else if (_moveDirection != Vector3.zero)
        {
            _follower.HandleSpriteFlip(_moveDirection.x);
        }
    }
}
