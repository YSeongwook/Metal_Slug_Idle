using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeroCollectionManager : Singleton<HeroCollectionManager>
{
    private List<HeroCollectionItem> heroCollection;
    private string fileName = "HeroCollection.json";

    protected override void Awake()
    {
        base.Awake();
        InitializeCollection();
        LoadCollection();
    }

    public bool HasHero(int heroId)
    {
        foreach (var hero in heroCollection)
        {
            if (hero.id == heroId)
            {
                return hero.owned;
            }
        }
        return false;
    }

    public void Initialize(int maxHeroes)
    {
        heroCollection = new List<HeroCollectionItem>();
        for (int i = 0; i < maxHeroes; i++)
        {
            heroCollection.Add(new HeroCollectionItem { id = i, owned = false });
        }
        SaveCollection();
    }

    public void AddHero(int heroId)
    {
        foreach (var hero in heroCollection)
        {
            if (hero.id == heroId)
            {
                hero.owned = true;
                SaveCollection();
                return;
            }
        }

        heroCollection.Add(new HeroCollectionItem { id = heroId, owned = true });
        SaveCollection();
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
            Initialize(10); // 파일이 없을 경우 초기화
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
            Initialize(10); // 파일이 없을 경우 초기화
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
        byte[] heroCollectionBytes = new byte[(heroCollection.Count + 7) / 8];
        foreach (var hero in heroCollection)
        {
            if (hero.owned)
            {
                int byteIndex = hero.id / 8;
                int bitIndex = hero.id % 8;
                heroCollectionBytes[byteIndex] |= (byte)(1 << bitIndex);
            }
        }
        return Convert.ToBase64String(heroCollectionBytes);
    }

    public void FromBase64(string base64)
    {
        byte[] heroCollectionBytes = Convert.FromBase64String(base64);
        heroCollection = new List<HeroCollectionItem>();
        for (int i = 0; i < heroCollectionBytes.Length * 8; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = i % 8;
            bool owned = (heroCollectionBytes[byteIndex] & (1 << bitIndex)) != 0;
            heroCollection.Add(new HeroCollectionItem { id = i, owned = owned });
        }
    }

    [Serializable]
    private class HeroCollectionWrapper
    {
        public List<HeroCollectionItem> heroCollection;
    }
}
