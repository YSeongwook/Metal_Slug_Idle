using UnityEngine;

public class FollowManualState : IFollowerState
{
    private FollowerController _follower;
    private Vector3 _targetPosition;
    private Vector3 _moveDirection;

    public void EnterState(FollowerController follower)
    {
        _follower = follower;
        _follower.Animator.SetBool(_follower.IsAttacking, false);
        _follower.CurrentTarget = null;
    }

    public void UpdateState()
    {
        CalculateDistance();
        SpriteFlip();
    }

    public void PhysicsUpdateState()
    {
        FollowLeader();
    }

    public void ExitState()
    {
        _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
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
}
