using UnityEngine;

[System.Serializable]
public class UserData
{
    public string userId;
    public string displayName;
    public int level;
    public string items;
    public long lastUpdated; // 타임스탬프

    public UserData(string userId, string displayName, int level, string items, long lastUpdated)
    {
        this.userId = userId;
        this.displayName = displayName;
        this.level = level;
        this.items = items;
        this.lastUpdated = lastUpdated;
    }
}

public static class JsonUtilityManager
{
    public static void SaveToJson<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName, json);
    }

    public static T LoadFromJson<T>(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        return default(T);
    }
}