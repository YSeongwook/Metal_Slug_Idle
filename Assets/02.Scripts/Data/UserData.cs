using UnityEngine;

[System.Serializable]
public class UserData
{
    public string userId;
    public string displayName;
    public int level;
    public string items;
    public long lastUpdated; // Unix 타임스탬프, 로컬과 서버 동기화를 위해 필요한 데이터

    // 기본 생성자
    public UserData() { }

    // 매개변수를 받는 생성자
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