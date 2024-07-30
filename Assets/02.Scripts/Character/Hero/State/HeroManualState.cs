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
        if (!_hero.IsUserControlled)
        {
            _hero.TransitionToState(_hero.IdleState);
        }
    }

    public void PhysicsUpdateState()
    {
        if (_hero.MoveInput == Vector2.zero)
        {
            _hero.Rb.velocity = Vector3.zero; // 이동 입력이 없으면 속도를 0으로 설정
            return;
        }
        
        Move2();
        _hero.LastUserInputTime = Time.time;
    }
    
    public void ExitState()
    {
        _hero.IsUserControlled = false;
        _hero.CurrentTarget = null;
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
    
    // Move 메서드 이동 방식 변경 중
    private void Move2()
    {
        Vector3 moveDirection = new Vector3(_hero.MoveInput.x, 0, _hero.MoveInput.y * _hero.zSpeedMultiplier);
        _hero.Rb.velocity = moveDirection * _hero.moveSpeed;
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