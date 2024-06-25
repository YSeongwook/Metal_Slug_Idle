using System.IO;
using UnityEngine;

public static class JsonUtilityManager
{
    public static void SaveToJson<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/" + fileName, json);
    }

    public static T LoadFromJson<T>(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        return default(T);
    }
}