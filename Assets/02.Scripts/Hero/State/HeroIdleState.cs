using UnityEngine;

// 캐릭터의 Idle 상태를 정의하는 클래스
public class HeroIdleState : IHeroState
{
    private HeroController _hero;
    private float _checkInterval = 1f; // 적 탐지 간격
    private float _lastCheckTime;

    public void EnterState(HeroController hero)
    {
        _hero = hero;
        _lastCheckTime = Time.time;
    }

    public void UpdateState()
    {
        if (Time.time - _lastCheckTime > _checkInterval)
        {
            _lastCheckTime = Time.time;
            FindClosestEnemy();
        }
    }

    public void ExitState()
    {
        // 상태를 나갈 때 수행할 작업이 있으면 여기에 작성
    }

    // 가장 가까운 적을 찾는 메서드
    private void FindClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(_hero.transform.position, Mathf.Infinity, _hero.EnemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(new Vector3(_hero.transform.position.x, 0, _hero.transform.position.z), new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z));
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            _hero.MoveToTarget(closestEnemy.position);
            _hero.TransitionToState(_hero.MoveState);
        }
    }
}