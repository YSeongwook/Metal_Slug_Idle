using System.Collections.Generic;
using Gpm.Ui;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    public InfiniteScroll infiniteScroll;

    private Vector2 padding;
    private Vector2 space;

    private void Start()
    {
        List<DungeonData> dungeons = DungeonDataLoader.LoadDungeonsFromJson();

        padding = new Vector2(50, 20);
        space = new Vector2(0, 20);
        
        // 캐릭터 데이터 추가
        infiniteScroll.InsertData(dungeons.ToArray(), true);
        infiniteScroll.SetPadding(padding);
        infiniteScroll.SetSpace(space);
        infiniteScroll.MoveToFirstData();
    }
}