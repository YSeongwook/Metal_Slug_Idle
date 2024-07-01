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
    public string heroCollectionBase64; // Base64로 인코딩된 바이트 배열

    // 기본 생성자
    public UserHeroCollection() { }

    // 매개변수를 받는 생성자
    public UserHeroCollection(string userId, string heroCollectionBase64)
    {
        this.userId = userId;
        this.heroCollectionBase64 = heroCollectionBase64;
    }
}