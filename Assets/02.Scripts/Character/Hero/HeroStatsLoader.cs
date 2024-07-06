using System.Collections.Generic;
using UnityEngine;

public class HeroStatsLoader : MonoBehaviour
{
    public List<HeroStats> heroes;

    private const string HeroStatsFileName = "HeroStats.json";

    private void Start()
    {
        LoadHeroStats();
    }

    public void LoadHeroStats()
    {
        heroes = DataLoader<HeroStats>.LoadDataFromJson(HeroStatsFileName);
    }

    public HeroStats GetHeroStatsById(int id)
    {
        return heroes.Find(hero => hero.id == id);
    }
}