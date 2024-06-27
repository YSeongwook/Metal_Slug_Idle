using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DungeonDataLoader
{
    public static List<DungeonData> LoadDungeonsFromJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "DungeonData.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            DungeonDataWrapper dungeonDataWrapper = JsonUtility.FromJson<DungeonDataWrapper>(json);
            return dungeonDataWrapper.dungeons;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return new List<DungeonData>();
        }
    }

    [Serializable]
    private class DungeonDataWrapper
    {
        public List<DungeonData> dungeons;
    }
}