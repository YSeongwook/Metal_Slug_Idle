using UnityEngine;

/**
 * @brief 팔로워 캐릭터의 동작을 정의하는 클래스입니다.
 */
public class FollowerController : HeroController
{
    public LeaderController leader;
    public Vector3 relativePosition;
    public float followDistance = 1f;

    private FollowerAttack _followerAttack;

    protected override void Awake()
    {
        base.Awake();
        _followerAttack = GetComponent<FollowerAttack>();
    }

    protected override void FixedUpdate()
    {
        if (leader.IsMoving)
        {
            FollowLeader();
        }
        else if (!_followerAttack.IsAttacking)
        {
            ReturnToPosition();
        }
        else
        {
            base.FixedUpdate();
        }
    }

    /**
     * @brief 리더를 따라가는 메서드
     */
    private void FollowLeader()
    {
        Vector3 leaderPosition = leader.transform.position + relativePosition;
        Vector3 direction = (leaderPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, leaderPosition);

        if (distance > followDistance)
        {
            Vector3 moveDirection = (leaderPosition - transform.position).normalized;
            Vector3 newPosition = Rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime);
            newPosition.y = Rb.position.y;

            Rb.MovePosition(newPosition);
            Animator.SetFloat(HeroController.Speed, moveDirection.magnitude);
        }
        else
        {
            Animator.SetFloat(HeroController.Speed, 0);
        }
    }

    /**
     * @brief 원래 위치로 돌아가는 메서드
     */
    private void ReturnToPosition()
    {
        Vector3 leaderPosition = leader.transform.position + relativePosition;
        Vector3 direction = (leaderPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, leaderPosition);

        if (distance > followDistance)
        {
            Vector3 moveDirection = (leaderPosition - transform.position).normalized;
            Vector3 newPosition = Rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime);
            newPosition.y = Rb.position.y;

            Rb.MovePosition(newPosition);
            Animator.SetFloat(HeroController.Speed, moveDirection.magnitude);
        }
        else
        {
            Animator.SetFloat(HeroController.Speed, 0);
        }
    }
}
