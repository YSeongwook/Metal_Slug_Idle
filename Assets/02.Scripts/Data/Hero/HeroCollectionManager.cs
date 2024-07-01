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
            Debug.Log($"Hero {heroId} owned: {heroCollection[heroId]}"); // 디버그 로그 추가
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
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
#else
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
#endif

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            File.WriteAllText(filePath, json);
        }
        else
        {
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
            if (!string.IsNullOrEmpty(json))
            {
                HeroCollectionWrapper wrapper = JsonUtility.FromJson<HeroCollectionWrapper>(json);
                if (wrapper != null && wrapper.heroCollection != null)
                {
                    heroCollection = new bool[wrapper.heroCollection.Length];
                    for (int i = 0; i < wrapper.heroCollection.Length; i++)
                    {
                        heroCollection[wrapper.heroCollection[i].id] = wrapper.heroCollection[i].owned;
                    }
                }
                else
                {
                    Initialize(15); // 데이터가 유효하지 않을 경우 초기화
                }
            }
            else
            {
                Initialize(15); // 파일이 비어 있을 경우 초기화
            }
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
        List<HeroOwnership> heroOwnershipList = new List<HeroOwnership>();
        for (int i = 0; i < heroCollection.Length; i++)
        {
            heroOwnershipList.Add(new HeroOwnership { id = i, owned = heroCollection[i] });
        }
        HeroCollectionWrapper wrapper = new HeroCollectionWrapper { heroCollection = heroOwnershipList.ToArray() };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    public string ToBase64()
    {
        List<HeroOwnership> heroOwnershipList = new List<HeroOwnership>();
        for (int i = 0; i < heroCollection.Length; i++)
        {
            heroOwnershipList.Add(new HeroOwnership { id = i, owned = heroCollection[i] });
        }
        string json = JsonUtility.ToJson(new HeroCollectionWrapper { heroCollection = heroOwnershipList.ToArray() });
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(jsonBytes);
    }

    public void FromBase64(string base64)
    {
        byte[] jsonBytes = Convert.FromBase64String(base64);
        string json = System.Text.Encoding.UTF8.GetString(jsonBytes);
        HeroCollectionWrapper wrapper = JsonUtility.FromJson<HeroCollectionWrapper>(json);
        heroCollection = new bool[wrapper.heroCollection.Length];
        for (int i = 0; i < wrapper.heroCollection.Length; i++)
        {
            heroCollection[wrapper.heroCollection[i].id] = wrapper.heroCollection[i].owned;
        }
    }

    [Serializable]
    private class HeroCollectionWrapper
    {
        public HeroOwnership[] heroCollection;
    }

    [Serializable]
    private class HeroOwnership
    {
        public int id;
        public bool owned;
    }
}
