using System.Collections.Generic;
using Gpm.Ui;
using UnityEngine;

public class HeroManager : Singleton<HeroManager>
{
    public InfiniteScroll infiniteScroll;

    private Vector2 padding;
    private Vector2 space;
    
    protected override void Awake()
    {
        base.Awake();
        padding = new Vector2(60, 30);
        space = new Vector2(120, 30);
    }
    
    private void Start()
    {
        LoadData();
    }

    private void SetPaddingAndSpace()
    {
        infiniteScroll.SetPadding(padding);
        infiniteScroll.SetSpace(space);
    }

    public void LoadData()
    {
        List<HeroData> heroes = HeroDataLoader.LoadHeroesFromJson();
        
        infiniteScroll.ClearData(); // 데이터 초기화
        infiniteScroll.InsertData(heroes.ToArray(), true); // 데이터 추가
        SetPaddingAndSpace(); // padding, space 적용
        infiniteScroll.MoveToFirstData();
    }
}