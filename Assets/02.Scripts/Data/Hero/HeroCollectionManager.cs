using System;
using UnityEngine;

public class HeroCollectionManager : Singleton<HeroCollectionManager>
{
    private byte[] heroCollection;

    protected override void Awake()
    {
        base.Awake();
        // 추가 초기화 코드가 필요하면 여기에 작성
    }

    public void Initialize(int maxHeroes)
    {
        heroCollection = new byte[(maxHeroes + 7) / 8];
    }

    // 영웅 추가
    public void AddHero(int heroId)
    {
        int byteIndex = heroId / 8;
        int bitIndex = heroId % 8;
        heroCollection[byteIndex] |= (byte)(1 << bitIndex);
    }

    // 영웅 소유 여부 확인
    public bool HasHero(int heroId)
    {
        int byteIndex = heroId / 8;
        int bitIndex = heroId % 8;
        return (heroCollection[byteIndex] & (1 << bitIndex)) != 0;
    }

    // Base64로 인코딩
    public string ToBase64()
    {
        return Convert.ToBase64String(heroCollection);
    }

    // Base64로부터 디코딩
    public void FromBase64(string base64)
    {
        heroCollection = Convert.FromBase64String(base64);
    }
}