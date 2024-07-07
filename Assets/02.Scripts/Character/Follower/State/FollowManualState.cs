using UnityEngine;

public class FollowManualState : IFollowerState
{
    private FollowerController _follower;

    public void EnterState(FollowerController follower)
    {
        _follower = follower;
        DebugLogger.Log("Enter FollowManualState");
    }

    public void UpdateState()
    {
        if (_follower.MoveInput == Vector2.zero)
        {
            _follower.TransitionToState(_follower.FollowState);
        }
    }

    public void PhysicsUpdateState()
    {
        if (_follower.MoveInput == Vector2.zero) return;
        
        Move();
        _follower.LastUserInputTime = Time.time;
    }
    
    public void ExitState()
    {
    }
    
    private void Move()
    {
        Vector3 moveDirection = new Vector3(_follower.MoveInput.x, 0, _follower.MoveInput.y * _follower.zSpeedMultiplier);
        Vector3 newPosition = _follower.Rb.position + moveDirection * (_follower.moveSpeed * Time.fixedDeltaTime);
        newPosition.y = _follower.InitialY;
        _follower.Rb.MovePosition(newPosition);
        _follower.Animator.SetFloat(_follower.SpeedParameter, moveDirection.magnitude);

        if (moveDirection != Vector3.zero)
        {
            _follower.HandleSpriteFlip(_follower.MoveInput.x);
        }
        else
        {
            _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
            _follower.TransitionToState(_follower.FollowState);
        }
    }
}