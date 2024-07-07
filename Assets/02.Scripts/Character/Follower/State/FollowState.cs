using UnityEngine;

public class FollowState : IFollowerState
{
    private FollowerController _follower;
    private const float CheckInterval = 0.5f; // 적 탐지 간격
    private float _lastCheckTime;
    private float originalMoveSpeed;
    
    private Vector3 _targetPosition;
    private Vector3 _moveDirection;

    public void EnterState(FollowerController follower)
    {
        _follower = follower;
        _lastCheckTime = Time.time;
        _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
        originalMoveSpeed = _follower.heroStats.moveSpeed; // 원래 이동 속도 저장

        // 현재 타겟이 없을 때만 포메이션 복귀 상태로 설정
        if (_follower.CurrentTarget == null)
        {
            _follower.heroStats.moveSpeed = originalMoveSpeed * 1.5f; // 이동 속도 1.5배로 설정
        }

        Debug.Log("Enter FollowState");
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
            FindClosestEnemy();
        }
        
        CalculateDistance();
        SpriteFlip();
    }

    public void PhysicsUpdateState()
    {
        FollowLeader();
    }

    public void ExitState()
    {
        _follower.heroStats.moveSpeed = originalMoveSpeed; // 이동 속도 원래대로 복원
    }
    
    private void FollowLeader()
    {
        if (_moveDirection.magnitude > 0.01f)
        {
            _follower.Rb.MovePosition(Vector3.MoveTowards(_follower.Rb.position, _targetPosition, _follower.heroStats.moveSpeed * 2f * Time.deltaTime));
            _follower.Animator.SetFloat(_follower.SpeedParameter, _moveDirection.magnitude);
        }
        else
        {
            _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
            _follower.heroStats.moveSpeed = originalMoveSpeed; // 위치 도달 시 이동 속도 원래대로 복원
            _moveDirection = Vector3.zero;  // 미세한 이동 멈춤
        }
    }
    
    private void CalculateDistance()
    {
        _targetPosition = _follower.leader.transform.position + _follower.formationOffset;
        _moveDirection = _targetPosition - _follower.transform.position;
    }

    private void SpriteFlip()
    {
        if (_moveDirection != Vector3.zero)
        {
            _follower.HandleSpriteFlip(_follower.MoveInput.x);
        }
    }

    private void FindClosestEnemy()
    {
        _follower.FindClosestEnemy();
        if (_follower.CurrentTarget != null)
        {
            float distanceToTarget = _follower.GetDistanceToTarget(_follower.CurrentTarget);
            if (distanceToTarget <= _follower.heroStats.attackRange)
            {
                _follower.TransitionToState(_follower.AttackState);
            }
        }
    }
}
