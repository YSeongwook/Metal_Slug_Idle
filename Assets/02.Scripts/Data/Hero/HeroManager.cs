using System.Collections.Generic;
using Gpm.Ui;
using UnityEngine;

public class HeroManager : Singleton<HeroManager>
{
    public InfiniteScroll infiniteScroll;

    private Vector2 padding;
    private Vector2 space;

    private void Start()
    {
        List<HeroData> heroes = HeroDataLoader.LoadHeroesFromJson();

        padding = new Vector2(60, 30);
        space = new Vector2(120, 30);
        
        // 캐릭터 데이터 추가
        infiniteScroll.InsertData(heroes.ToArray(), true);
        infiniteScroll.SetPadding(padding);
        infiniteScroll.SetSpace(space);
    }
}