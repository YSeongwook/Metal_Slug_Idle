using UnityEngine;

public class FollowerController : HeroController
{
    public LeaderController leader;
    public Vector3 relativePosition;
    public float followDistance = 1f;

    private HeroStatsManager _heroStats;

    protected override void Awake()
    {
        base.Awake();
        _heroStats = GetHeroStatsManager();
        CalculateRelativePosition();
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
            Vector3 newPosition = Rb.position + moveDirection * (_heroStats.MoveSpeed * Time.fixedDeltaTime);
            newPosition.y = Rb.position.y;

            Rb.MovePosition(newPosition);
            Animator.SetFloat(SpeedParameter, moveDirection.magnitude);
        }
        else
        {
            Animator.SetFloat(SpeedParameter, 0);
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
            Vector3 newPosition = Rb.position + moveDirection * (_heroStats.MoveSpeed * Time.fixedDeltaTime);
            newPosition.y = Rb.position.y;

            Rb.MovePosition(newPosition);
            Animator.SetFloat(SpeedParameter, moveDirection.magnitude);
        }
        else
        {
            Animator.SetFloat(SpeedParameter, 0);
        }
    }

    private void Update()
    {
        FollowLeader();
    }
}
