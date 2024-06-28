using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SummonDataLoader
{
    public static List<SummonData> LoadSummonsFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "SummonData.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SummonDataWrapper summonDataWrapper = JsonUtility.FromJson<SummonDataWrapper>(json);
            return summonDataWrapper.summons;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return new List<SummonData>();
        }
    }

    [Serializable]
    private class SummonDataWrapper
    {
        public List<SummonData> summons;
    }
}