using UnityEngine;

public class LeaderAttack : HeroAttack
{
    protected override void Update()
    {
        if (_heroController.IsUserControlled || _isPaused) return; // 유저가 조작 중이거나 일시 중지된 경우 자동 공격 중지

        base.Update(); // 기본 로직 실행
    }

    protected override void HandleMovementAndAttack()
    {
        base.HandleMovementAndAttack(); // 기본 로직 실행
    }

    protected override void Attack()
    {
        base.Attack(); // 기본 로직 실행
    }
}