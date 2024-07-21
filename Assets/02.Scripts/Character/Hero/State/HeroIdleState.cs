using UnityEngine;

public class HeroIdleState : IHeroState
{
    private HeroController _hero;
    private const float CheckInterval = 0.5f; // 적 탐지 간격
    private float _lastCheckTime;

    public void EnterState(HeroController hero)
    {
        _hero = hero;
        _lastCheckTime = Time.time;
        _hero.Rb.velocity = Vector3.zero;
        _hero.Animator.SetFloat(_hero.SpeedParameter, 0);
    }

    public void UpdateState()
    {
        if (Time.time - _lastCheckTime >= CheckInterval)
        {
            _lastCheckTime = Time.time;
            FindClosestEnemy();
        }
        
        if(!_hero.IsAutoMode) _hero.Rb.velocity = Vector3.zero;
    }

    public void PhysicsUpdateState()
    {
        
    }

    public void ExitState() { }

    private void FindClosestEnemy()
    {
        _hero.FindClosestEnemy();
        
        if (_hero.CurrentTarget == null) return;
        
        float distanceToTarget = _hero.GetDistanceToTarget(_hero.CurrentTarget);
        if (distanceToTarget <= _hero.heroStats.attackRange)
        {
            _hero.TransitionToState(_hero.AttackState);
        }
        else if (_hero.IsAutoMode)
        {
            _hero.TransitionToState(_hero.MoveState);
        }
    }
}