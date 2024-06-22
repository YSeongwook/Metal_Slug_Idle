using System.Collections.Generic;
using UnityEngine;

public class HeroDataLoaderTest : MonoBehaviour
{
    private void Start()
    {
        List<HeroData> heroes = HeroDataLoader.LoadHeroesFromJson();

        if (heroes == null) return;
        
        foreach (var hero in heroes)
        {
            Debug.Log($"Name: {hero.name}, Grade: {hero.grade}, Level: {hero.level}, Health: {hero.health}, Attack: {hero.attack}, PortraitPath: {hero.portraitPath}");
        }
    }
}