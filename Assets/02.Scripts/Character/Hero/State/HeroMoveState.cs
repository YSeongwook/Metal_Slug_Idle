using UnityEngine;

public class HeroMoveState : IHeroState
{
    private HeroController _hero;
    private Vector3 _targetPosition;
    private const float MaxDistanceFromLeader = 5f; // 리더로부터의 최대 거리

    public void EnterState(HeroController hero)
    {
        _hero = hero;
        
        // DebugLogger.Log("Enter MoveState");
    }

    public void UpdateState()
    {
        
    }

    public void PhysicsUpdateState()
    {
        if (!_hero.IsAutoMode) return;
        
        MoveToTarget();
    }

    public void ExitState() { }

    public void SetTarget(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    private void MoveToTarget()
    {
        if (_hero is FollowerController follower)
        {
            if (Vector3.Distance(_hero.transform.position, follower.leader.transform.position) > MaxDistanceFromLeader)
            {
                _targetPosition = follower.leader.transform.position;
            }
        }

        Vector3 direction = (_targetPosition - _hero.transform.position).normalized;
        Vector3 moveDirection = new Vector3(direction.x, 0, direction.z * _hero.heroStats.moveSpeed);
        Vector3 newPosition = _hero.Rb.position + moveDirection * (_hero.heroStats.moveSpeed * Time.fixedDeltaTime);
        newPosition.y = _hero.InitialY;
        _hero.Rb.MovePosition(newPosition);

        if (moveDirection != Vector3.zero)
        {
            _hero.HandleSpriteFlip(direction.x);
            _hero.Animator.SetFloat(_hero.SpeedParameter, moveDirection.magnitude);
        }
        else
        {
            _hero.Animator.SetFloat(_hero.SpeedParameter, 0);
            _hero.TransitionToState(_hero.IdleState);
        }
    }
}