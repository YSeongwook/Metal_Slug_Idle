using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeroCollectionManager : Singleton<HeroCollectionManager>
{
    private bool[] heroCollection; // 보유 상태를 저장하는 배열
    private string fileName = "HeroCollection.json";

    protected override void Awake()
    {
        base.Awake();
        InitializeCollection();
        LoadCollection();
    }

    public bool HasHero(int heroId)
    {
        if (heroId >= 0 && heroId < heroCollection.Length)
        {
            return heroCollection[heroId];
        }
        return false;
    }

    public void Initialize(int maxHeroes)
    {
        heroCollection = new bool[maxHeroes];
        SaveCollection();
    }

    public void AddHero(int heroId)
    {
        if (heroId >= 0 && heroId < heroCollection.Length)
        {
            heroCollection[heroId] = true;
            SaveCollection();
        }
    }

    public void UpdateHeroCollection(int[] heroIds)
    {
        foreach (int heroId in heroIds)
        {
            AddHero(heroId);
        }
    }

    private void InitializeCollection()
    {
#if UNITY_EDITOR
        string persistentFilePath = Path.Combine(Application.streamingAssetsPath, fileName);
#else
        string persistentFilePath = Path.Combine(Application.persistentDataPath, fileName);
#endif
        string streamingFilePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(streamingFilePath) && !File.Exists(persistentFilePath))
        {
            string json = File.ReadAllText(streamingFilePath);
            File.WriteAllText(persistentFilePath, json);
        }
        else if (!File.Exists(persistentFilePath))
        {
            Debug.LogError("StreamingAssets에 기본 HeroCollection.json 파일이 없습니다.");
            Initialize(15); // 파일이 없을 경우 초기화
        }
    }

    private void LoadCollection()
    {
#if UNITY_EDITOR
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
#else
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
#endif
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            HeroCollectionWrapper wrapper = JsonUtility.FromJson<HeroCollectionWrapper>(json);
            heroCollection = wrapper.heroCollection;
        }
        else
        {
            Initialize(15); // 파일이 없을 경우 초기화
        }
    }

    public void SaveCollection()
    {
#if UNITY_EDITOR
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
#else
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
#endif
        HeroCollectionWrapper wrapper = new HeroCollectionWrapper { heroCollection = heroCollection };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    public string ToBase64()
    {
        string json = JsonUtility.ToJson(new HeroCollectionWrapper { heroCollection = heroCollection });
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(jsonBytes);
    }

    public void FromBase64(string base64)
    {
        byte[] jsonBytes = Convert.FromBase64String(base64);
        string json = System.Text.Encoding.UTF8.GetString(jsonBytes);
        HeroCollectionWrapper wrapper = JsonUtility.FromJson<HeroCollectionWrapper>(json);
        heroCollection = wrapper.heroCollection;
    }

    [Serializable]
    private class HeroCollectionWrapper
    {
        public bool[] heroCollection;
    }
}
