using System.Collections.Generic;
using UnityEngine;
using Gpm.Ui;

public class HeroManager : MonoBehaviour
{
    public InfiniteScroll infiniteScroll;
    public Sprite[] rankSprites; // D, C, B, A, S 순서로 스프라이트 배열
    public Sprite[] typeSprites; // 근거리형, 원거리형, 방어형, 지원형 순서로 스프라이트 배열

    private void Start()
    {
        List<HeroData> heroes = HeroDataLoader.LoadHeroesFromJson();

        // HeroListItem 프리팹 설정
        foreach (var item in infiniteScroll.GetComponentsInChildren<HeroListItem>())
        {
            item.rankSprites = rankSprites;
            item.typeSprites = typeSprites;
        }

        // 캐릭터 데이터 추가
        infiniteScroll.InsertData(heroes.ToArray(), true);
    }
}