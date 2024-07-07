using UnityEngine;

public class FollowState : IFollowerState
{
    private FollowerController _follower;
    private const float CheckInterval = 0.5f; // 적 탐지 간격
    private float _lastCheckTime;

    public void EnterState(FollowerController follower)
    {
        _follower = follower;
        _lastCheckTime = Time.time;
        _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
        
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
    }

    public void PhysicsUpdateState()
    {
        FollowLeader();
    }

    public void ExitState() { }

    private void FollowLeader()
    {
        Vector3 targetPosition = _follower.leader.transform.position + _follower.formationOffset;
        float distance = Vector3.Distance(_follower.transform.position, targetPosition);

        if (distance > _follower.followDistance)
        {
            Vector3 direction = (targetPosition - _follower.transform.position).normalized;
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.z) * _follower.heroStats.moveSpeed;
            Vector3 newPosition = _follower.Rb.position + moveDirection * Time.deltaTime;
            newPosition.y = _follower.Rb.position.y;

            _follower.Rb.MovePosition(newPosition);
            _follower.Animator.SetFloat(_follower.SpeedParameter, moveDirection.magnitude);
        }
        else
        {
            _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
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
