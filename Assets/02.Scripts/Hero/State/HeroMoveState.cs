using UnityEngine;

public class HeroMoveState : IHeroState
{
    private HeroController _hero;
    private Vector3 _targetPosition;

    public void EnterState(HeroController hero)
    {
        _hero = hero;
    }

    public void UpdateState()
    {
        MoveToTarget();
    }

    public void ExitState()
    {
        // 상태를 나갈 때 수행할 작업이 있으면 여기에 작성합니다.
    }

    public void SetTarget(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    private void MoveToTarget()
    {
        Vector3 direction = (_targetPosition - _hero.transform.position).normalized;
        Vector3 moveDirection = new Vector3(direction.x, 0, direction.z * _hero.zSpeedMultiplier);
        Vector3 newPosition = _hero.Rb.position + moveDirection * (_hero.moveSpeed * Time.fixedDeltaTime);
        newPosition.y = _hero.InitialY;
        _hero.Rb.MovePosition(newPosition);
        _hero.Animator.SetFloat(HeroController.Speed, moveDirection.magnitude);

        float distanceToTarget = Vector3.Distance(new Vector3(_hero.transform.position.x, 0, _hero.transform.position.z), new Vector3(_targetPosition.x, 0, _targetPosition.z));

        if (distanceToTarget < 1f)
        {
            _hero.StopMoving();
            _hero.TransitionToState(_hero.IdleState);
        }
        else if (distanceToTarget <= _hero.HeroStatsManager.AttackRange)
        {
            _hero.StopMoving();
            _hero.TransitionToState(_hero.AttackState);
        }

        if (moveDirection != Vector3.zero)
        {
            _hero.HandleSpriteFlip(direction.x);
        }
    }
}