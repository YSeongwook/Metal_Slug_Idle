using System;
using UnityEngine;

public class HeroStatsManager : MonoBehaviour
{
    public int heroId; // 로드할 영웅의 ID
    public HeroStats heroStats;
    
    private HeroStatsLoader heroStatsLoader;

    private void Awake()
    {
        heroStatsLoader = FindObjectOfType<HeroStatsLoader>();
        Initialize();
    }
    
    public void Initialize()
    {
        if (heroStatsLoader != null)
        {
            heroStatsLoader.LoadHeroStats();
            heroStats = heroStatsLoader.GetHeroStatsById(heroId);
        }

        if (heroStats == null)
        {
            Debug.LogError("HeroStats could not be loaded.");
        }
    }

    public float AttackRange => heroStats.attackRange;
    public float AttackDamage => heroStats.attack;
    public float AttackSpeed => heroStats.attackSpeed;
    public float MoveSpeed => heroStats.moveSpeed;

    // 체력 회복 함수 호출 예시
    public void HealHero(int amount)
    {
        heroStats.Heal(amount);
    }

    // 데미지 처리 함수 호출 예시
    public void DamageHero(int damage)
    {
        heroStats.TakeDamage(damage);
        if (heroStats.health <= 0)
        {
            // 사망 처리 (예: 애니메이션, 게임 오버 등)
            // 예: GetComponent<Animator>().SetTrigger("Die");
        }
    }

    public HeroStats GetHeroStats()
    {
        return heroStats;
    }
}