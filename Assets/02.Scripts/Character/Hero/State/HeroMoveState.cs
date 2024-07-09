using UnityEngine;

public class HeroMoveState : IHeroState
{
    private HeroController _hero;
    private Transform _target;

    public void EnterState(HeroController hero)
    {
        _hero = hero;
        SetTarget();
    }

    public void UpdateState()
    {
        if(_hero.CurrentTarget == null) _hero.TransitionToState(_hero.IdleState);
    }

    public void PhysicsUpdateState()
    {
        if (!_hero.IsAutoMode) return;
        
        DebugLogger.Log("AutoMode");
        
        MoveTowardsTarget();
    }

    public void ExitState() { }

    private void SetTarget()
    {
        _target = _hero.CurrentTarget;
    }
    
    private void MoveTowardsTarget()
    {
        if (_target == null) return;

        DebugLogger.Log(_target);
        
        Vector3 direction = (_target.position - _hero.transform.position).normalized;
        float attackRangeBuffer = 1.0f; // 추가적인 마진
        float distanceToMove = Vector3.Distance(_hero.transform.position, _target.position) - (_hero.heroStats.attackRange - attackRangeBuffer);

        if (distanceToMove > 0)
        {
            Vector3 moveDirection = direction * (_hero.heroStats.moveSpeed * Time.deltaTime);
            Vector3 newPosition = _hero.Rb.position + moveDirection;

            _hero.Rb.MovePosition(newPosition);
            _hero.Animator.SetFloat(_hero.SpeedParameter, moveDirection.magnitude);
        }
        else
        {
            _hero.Animator.SetFloat(_hero.SpeedParameter, 0);
            _hero.TransitionToState(_hero.AttackState);
        }
    }
}