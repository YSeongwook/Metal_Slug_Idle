using UnityEngine;

public class HeroMoveState : IHeroState
{
    private HeroController _hero;
    private Vector3 _targetPosition;
    private const float MaxDistanceFromLeader = 5f; // 리더로부터의 최대 거리

    public void EnterState(HeroController hero)
    {
        _hero = hero;
        
        DebugLogger.Log("Enter MoveState");
    }

    public void UpdateState()
    {
        if (!_hero.IsAutoMode) return; // 오토 모드가 아닌 경우 이동하지 않음

        MoveToTarget();
    }
    
    public void PhysicsUpdateState()
    {
        
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
        if (_hero is FollowerController follower)
        {
            if (Vector3.Distance(_hero.transform.position, follower.leader.transform.position) > MaxDistanceFromLeader)
            {
                _targetPosition = follower.leader.transform.position; // 리더의 위치로 돌아가기
            }
        }

        Vector3 direction = (_targetPosition - _hero.transform.position).normalized;
        Vector3 moveDirection = new Vector3(direction.x, 0, direction.z * _hero.heroStats.moveSpeed);
        Vector3 newPosition = _hero.Rb.position + moveDirection * (_hero.heroStats.moveSpeed * Time.fixedDeltaTime);
        newPosition.y = _hero.InitialY;
        _hero.Rb.MovePosition(newPosition);
        _hero.Animator.SetFloat(_hero.SpeedParameter, moveDirection.magnitude);

        float distanceToTarget = Vector3.Distance(new Vector3(_hero.transform.position.x, 0, _hero.transform.position.z), new Vector3(_targetPosition.x, 0, _targetPosition.z));

        if (distanceToTarget < 1f)
        {
            _hero.StopMoving();
            _hero.TransitionToState(_hero.IdleState);
        }
        else if (distanceToTarget <= _hero.heroStats.attackRange)
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
