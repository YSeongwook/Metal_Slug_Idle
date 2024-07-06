using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroStats
{
    public int id; // 캐릭터 id
    public string name; // 캐릭터 이름
    public string rank; // 캐릭터 등급 (D, C, B, A, S)
    public int level; // 캐릭터 레벨
    public int health; // 체력
    public int maxHealth; // 최대 체력
    public int attack; // 공격력
    public int defense; // 방어력
    public float moveSpeed; // 이동 속도
    public float attackSpeed; // 공격 속도
    public float attackRange; // 공격 범위
    public int experience; // 현재 경험치
    public int maxExperience; // 레벨업에 필요한 경험치
    public float criticalHitChance; // 크리티컬 확률
    public float criticalDamageMultiplier; // 크리티컬 데미지 배수
    public List<string> skills; // 스킬 목록

    // 체력 회복 메서드
    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    // 데미지 처리 메서드
    public void TakeDamage(int amount)
    {
        int damage = Mathf.Max(amount - defense, 0);
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // 레벨업 메서드
    public void GainExperience(int amount)
    {
        experience += amount;
        if (experience >= maxExperience)
        {
            LevelUp();
        }
    }

    // 레벨업 처리
    private void LevelUp()
    {
        level++;
        experience = 0;
        maxExperience += 50; // 예시로, 레벨업 시 필요한 경험치 증가
        // 추가적인 레벨업 보너스 처리 가능 (체력, 공격력 증가 등)
    }

    // 캐릭터 사망 처리
    private void Die()
    {
        // 사망 처리 로직 (예: 애니메이션 재생, 게임 오버 등)
        Debug.Log($"{name} has died.");
    }
}


