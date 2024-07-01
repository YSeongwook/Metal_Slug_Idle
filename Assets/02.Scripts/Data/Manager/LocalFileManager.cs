using System.IO;
using UnityEngine;

public class LocalFileManager : Singleton<LocalFileManager>
{
    private string heroCollectionFileName = "HeroCollection.json";

    public void SaveHeroCollectionToLocalFile(string base64HeroCollection)
    {
        string json = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64HeroCollection));
        string path = Path.Combine(Application.persistentDataPath, heroCollectionFileName);
        File.WriteAllText(path, json);
        Debug.Log("HeroCollection saved to local file.");
    }

    public string LoadHeroCollectionFromLocalFile()
    {
        string path = Path.Combine(Application.persistentDataPath, heroCollectionFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        }
        else
        {
            Debug.LogWarning("HeroCollection local file not found.");
            return null;
        }
    }

    public void DeleteLocalHeroCollectionFile()
    {
        string path = Path.Combine(Application.persistentDataPath, heroCollectionFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("HeroCollection local file deleted.");
        }
    }
}