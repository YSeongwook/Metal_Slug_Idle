using System;

[Serializable]
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

[Serializable]
public class UserHeroCollection
{
    public string userId;
    public byte[] heroCollection; // 유저가 소유한 캐릭터 정보

    // 기본 생성자
    public UserHeroCollection() { }

    // 매개변수를 받는 생성자
    public UserHeroCollection(string userId, byte[] heroCollection)
    {
        this.userId = userId;
        this.heroCollection = heroCollection;
    }
}