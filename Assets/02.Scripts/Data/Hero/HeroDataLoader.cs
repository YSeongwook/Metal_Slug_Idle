using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class HeroDataLoader
{
    public static List<HeroData> LoadHeroesFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "heroData.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            HeroDataWrapper heroDataWrapper = JsonUtility.FromJson<HeroDataWrapper>(json);
            return heroDataWrapper.heroes;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return new List<HeroData>();
        }
    }

    [Serializable]
    private class HeroDataWrapper
    {
        public List<HeroData> heroes;
    }
}