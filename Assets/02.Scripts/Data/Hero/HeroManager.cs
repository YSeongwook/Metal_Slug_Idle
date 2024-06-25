using System.Collections.Generic;
using UnityEngine;
using Gpm.Ui;

public class HeroManager : MonoBehaviour
{
    public InfiniteScroll infiniteScroll;
    public Sprite[] rankSprites; // D, C, B, A, S 순서로 스프라이트 배열
    public Sprite[] typeSprites; // 근거리형, 원거리형, 방어형, 지원형 순서로 스프라이트 배열

    private Vector2 padding;
    private Vector2 space;

    private void Start()
    {
        List<HeroData> heroes = HeroDataLoader.LoadHeroesFromJson();

        // HeroListItem 프리팹 설정
        foreach (var item in infiniteScroll.GetComponentsInChildren<HeroListItem>())
        {
            item.rankSprites = rankSprites;
            item.typeSprites = typeSprites;
        }

        padding = new Vector2(60, 30);
        space = new Vector2(120, 30);
        
        // 캐릭터 데이터 추가
        infiniteScroll.InsertData(heroes.ToArray(), true);
        infiniteScroll.SetPadding(padding);
        infiniteScroll.SetSpace(space);
    }
}