using UnityEngine;

public class FollowState : IFollowerState
{
    private FollowerController _follower;

    public void EnterState(FollowerController follower)
    {
        _follower = follower;
        // 추가 초기화 코드가 필요한 경우 여기에 작성
        DebugLogger.Log("Enter FollowState");
    }

    public void UpdateState()
    {
        
    }

    public void PhysicsUpdateState()
    {
        FollowLeader();
    }

    public void ExitState()
    {
        // 종료 시 처리 코드가 필요한 경우 여기에 작성
    }

    private void FollowLeader()
    {
        Vector3 targetPosition = _follower.leader.transform.position + _follower.formationOffset;
        float distance = Vector3.Distance(_follower.transform.position, targetPosition);

        if (distance > _follower.followDistance)
        {
            Vector3 direction = (targetPosition - _follower.transform.position).normalized;
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.z) * _follower.heroStats.moveSpeed;
            Vector3 newPosition = _follower.Rb.position + moveDirection * Time.deltaTime;
            newPosition.y = _follower.Rb.position.y;

            _follower.Rb.MovePosition(newPosition);
            _follower.Animator.SetFloat(_follower.SpeedParameter, moveDirection.magnitude);
        }
        else
        {
            _follower.Animator.SetFloat(_follower.SpeedParameter, 0);
        }

        // 리더의 공격 상태를 이벤트를 통해 처리
        // _follower.TransitionToState(_follower.AttackState);
    }
}