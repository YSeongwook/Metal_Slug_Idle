using System.Collections.Generic;
using Gpm.Ui;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    public InfiniteScroll infiniteScroll;

    private Vector2 padding;
    private Vector2 space;

    protected override void Awake()
    {
        base.Awake();
        padding = new Vector2(50, 20);
        space = new Vector2(0, 20);
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
        List<DungeonData> dungeons = DungeonDataLoader.LoadDungeonsFromJson();
        
        infiniteScroll.ClearData(); // 데이터 초기화
        infiniteScroll.InsertData(dungeons.ToArray(), true); // 데이터 추가
        SetPaddingAndSpace(); // padding, space 적용
        infiniteScroll.MoveToFirstData();
    }
}