using UnityEngine;

public class HeroManualState : IHeroState
{
    private HeroController _hero;

    public void EnterState(HeroController hero)
    {
        _hero = hero;
    }

    public void UpdateState()
    {
        /*
        if (_hero.MoveInput == Vector2.zero)
        {
            DebugLogger.Log("TransitionIdleState");
            _hero.TransitionToState(_hero.IdleState);
        }
        */

        if (!_hero.IsUserControlled)
        {
            _hero.TransitionToState(_hero.IdleState);
        }
    }

    public void PhysicsUpdateState()
    {
        if (_hero.MoveInput == Vector2.zero) return;
        
        Move();
        _hero.LastUserInputTime = Time.time;
    }
    
    public void ExitState()
    {
    }
    
    private void Move()
    {
        Vector3 moveDirection = new Vector3(_hero.MoveInput.x, 0, _hero.MoveInput.y * _hero.zSpeedMultiplier);
        Vector3 newPosition = _hero.Rb.position + moveDirection * (_hero.moveSpeed * Time.fixedDeltaTime);
        newPosition.y = _hero.InitialY;
        _hero.Rb.MovePosition(newPosition);
        _hero.Animator.SetFloat(_hero.SpeedParameter, moveDirection.magnitude);

        if (moveDirection != Vector3.zero)
        {
            _hero.HandleSpriteFlip(_hero.MoveInput.x);
        }
        else
        {
            _hero.Animator.SetFloat(_hero.SpeedParameter, 0);
            _hero.TransitionToState(_hero.IdleState);
        }
    }
}