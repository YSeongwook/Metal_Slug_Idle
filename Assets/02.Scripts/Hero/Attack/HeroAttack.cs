using UnityEngine;

public class HeroAttack : MonoBehaviour
{
    public LayerMask enemyLayer;

    protected Animator _animator;
    protected HeroController _heroController;
    protected HeroStatsManager _heroStatsManager;
    protected Transform targetEnemy;
    protected float _lastAttackTime;
    protected bool _isPaused;
    public bool IsAttacking { get; protected set; }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _heroController = GetComponent<HeroController>();
        _heroStatsManager = GetComponent<HeroStatsManager>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    protected virtual void Update()
    {
        if (_heroController.IsUserControlled || _isPaused) return;

        FindClosestEnemy();
        HandleMovementAndAttack();
    }
    
    protected virtual void FindClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, Mathf.Infinity, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z));
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        targetEnemy = closestEnemy;
    }
    
    protected virtual void HandleMovementAndAttack()
    {
        if (targetEnemy == null) return;

        float distanceToEnemy = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetEnemy.position.x, 0, targetEnemy.position.z));

        if (distanceToEnemy > _heroStatsManager.AttackRange)
        {
            // _heroController.MoveToTarget(targetEnemy.position);
        }
        else
        {
            _heroController.StopMoving();
            Attack();
        }
    }
    
    protected virtual void Attack()
    {
        if (Time.time - _lastAttackTime < 1f / _heroStatsManager.AttackSpeed) return;

        _animator.SetTrigger(_heroController.AttackParameter);
        _lastAttackTime = Time.time;
        IsAttacking = true;

        if (targetEnemy != null)
        {
            // 적에게 데미지 입히기
            // targetEnemy.GetComponent<Enemy>().TakeDamage(_heroStatsManager.AttackDamage);
        }

        // 일정 시간 후 IsAttacking을 false로 설정
        Invoke(nameof(ResetAttack), 1f / _heroStatsManager.AttackSpeed);
    }

    private void ResetAttack()
    {
        IsAttacking = false;
    }

    public void PauseAttack()
    {
        _isPaused = true;
        IsAttacking = false;
    }

    public void ResumeAttack()
    {
        _isPaused = false;
    }
}
