using System.Collections.Generic;
using UnityEngine;

public class HeroDataManager : DataManager<HeroData>
{
    protected override void Start()
    {
        LoadData();
    }

    // 데이터를 로드하고 스크롤 뷰에 추가하는 메서드
    public override void LoadData()
    {
        List<HeroData> allHeroes = DataLoader<HeroData>.LoadDataFromJson(fileName);
        List<HeroData> ownedHeroes = new List<HeroData>();

        // 보유한 영웅 필터링
        foreach (HeroData hero in allHeroes)
        {
            if (HeroCollectionManager.Instance.HasHero(hero.id))
            {
                ownedHeroes.Add(hero);
            }
        }

        // InfiniteScroll 업데이트
        infiniteScroll.ClearData();
        infiniteScroll.InsertData(ownedHeroes.ToArray(), true);
        SetPaddingAndSpace();
        infiniteScroll.MoveToFirstData();
    }

    // 패딩과 스페이스 설정 메서드
    private void SetPaddingAndSpace()
    {
        infiniteScroll.SetPadding(padding);
        infiniteScroll.SetSpace(space);
    }
}