using UnityEngine;

public class FollowState : IFollowerState
{
    private FollowerController _follower;
    private float _originalMoveSpeed;
    
    private Vector3 _targetPosition;
    private Vector3 _moveDirection;

    public void EnterState(FollowerController follower)
    {
        _follower = follower;
        _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
        _originalMoveSpeed = _follower.heroStats.moveSpeed; // 원래 이동 속도 저장
        // 현재 타겟이 없을 때만 포메이션 복귀 상태로 설정
        if (_follower.CurrentTarget == null)
        {
            _follower.heroStats.moveSpeed = _originalMoveSpeed * 1.5f; // 이동 속도 1.5배로 설정
        }
    }

    public void UpdateState()
    {
        // 리더 공격 상태 들어가면 공격 상태로 전환

        if (_follower.leader.CurrentState is HeroAttackState)
        {
            _follower.TransitionToState(_follower.AttackState);
        }
        
        CalculateDistance();
        SpriteFlip();
    }

    public void PhysicsUpdateState()
    {
        FollowLeader();
    }

    public void ExitState()
    {
        _follower.heroStats.moveSpeed = _originalMoveSpeed; // 이동 속도 원래대로 복원
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
            _follower.heroStats.moveSpeed = _originalMoveSpeed; // 위치 도달 시 이동 속도 원래대로 복원
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
            _follower.HandleSpriteFlip(_moveDirection.x);
        }
    }
    
    // 팔로우 상태에서는 리더와 항상 같은 방향이어야함
    // TODO: 리더 스프라이트 변경 시 이벤트 발생 시켜서 같이 변하게 수정
}
