using System;
using System.IO;
using System.Text;
using UnityEngine;

public class HeroCollectionManager : Singleton<HeroCollectionManager>
{
    private HeroOwnership[] _heroCollection; // 보유 및 배치 상태를 저장하는 배열
    private const string FileName = "HeroCollection.json";

    protected override void Awake()
    {
        base.Awake();
        InitializeCollection();
        LoadCollection();
    }
    
    public bool HasHero(int heroId)
    {
        if (heroId >= 0 && heroId < _heroCollection.Length)
        {
            return _heroCollection[heroId].owned;
        }
        return false;
    }
    
    public bool IsHeroAssigned(int heroId)
    {
        if (heroId >= 0 && heroId < _heroCollection.Length)
        {
            return _heroCollection[heroId].assigned;
        }
        return false;
    }
    
    public void Initialize(int maxHeroes)
    {
        _heroCollection = new HeroOwnership[maxHeroes];
        for (int i = 0; i < maxHeroes; i++)
        {
            _heroCollection[i] = new HeroOwnership { id = i, owned = false, assigned = false };
        }
        SaveCollection();
    }
    
    public void AddHero(int heroId)
    {
        if (heroId >= 0 && heroId < _heroCollection.Length)
        {
            _heroCollection[heroId].owned = true;
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
        string filePath = Path.Combine(Application.persistentDataPath, FileName);

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
        string filePath = Path.Combine(Application.persistentDataPath, FileName);
        
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    HeroCollectionWrapper wrapper = JsonUtility.FromJson<HeroCollectionWrapper>(json);
                    if (wrapper != null && wrapper.heroCollection != null)
                    {
                        _heroCollection = wrapper.heroCollection;
                    }
                    else
                    {
                        Initialize(15); // 데이터가 유효하지 않을 경우 초기화
                    }
                }
                catch (Exception ex)
                {
                    Initialize(15); // JSON 파싱 중 오류 발생 시 초기화
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

    private void SaveCollection()
    {
        string filePath = Path.Combine(Application.persistentDataPath, FileName);
        
        HeroCollectionWrapper wrapper = new HeroCollectionWrapper { heroCollection = _heroCollection };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    public string ToBase64()
    {
        string json = JsonUtility.ToJson(new HeroCollectionWrapper { heroCollection = _heroCollection });
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(jsonBytes);
    }

    public void FromBase64(string base64)
    {
        byte[] jsonBytes = Convert.FromBase64String(base64);
        string json = Encoding.UTF8.GetString(jsonBytes);
        try
        {
            HeroCollectionWrapper wrapper = JsonUtility.FromJson<HeroCollectionWrapper>(json);
            _heroCollection = wrapper.heroCollection;
        }
        catch (Exception ex)
        {
            Initialize(15); // JSON 파싱 중 오류 발생 시 초기화
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
        public bool assigned;
    }
}
