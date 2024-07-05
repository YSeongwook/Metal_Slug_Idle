using UnityEngine;
using UnityEngine.InputSystem;

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
        CalculateRelativePosition();
    }

    protected override void FixedUpdate()
    {
        if (IsUserControlled)
        {
            // 유저가 조작 중인 경우 위치를 유지하며 이동
            Move();
            CalculateRelativePosition();
        }
        else if (leader.IsMoving)
        {
            FollowLeader();
        }
        else if (leader.CurrentState is HeroAttackState && !_followerAttack.IsAttacking)
        {
            AdjustPositionForAttack();
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
    
    private void CalculateRelativePosition()
    {
        relativePosition = transform.position - leader.transform.position;
    }
    
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
    
    private void AdjustPositionForAttack()
    {
        Vector3 leaderPosition = leader.transform.position + relativePosition;
        float distanceToLeader = Vector3.Distance(transform.position, leaderPosition);
        float attackRange = HeroStatsManager.AttackRange;

        if (distanceToLeader > attackRange)
        {
            MoveToTarget(leaderPosition);
        }
        else
        {
            StopMoving();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        base.OnMove(context);
        if (IsUserControlled)
        {
            CalculateRelativePosition(); // 상대 위치 갱신
        }
    }
}
