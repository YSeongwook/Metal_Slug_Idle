using System.Collections.Generic;
using Gpm.Ui;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public InfiniteScroll infiniteScroll;
    public Sprite frameD;
    public Sprite frameC;
    public Sprite frameB;
    public Sprite frameA;
    public Sprite frameS;

    private void Start()
    {
        List<HeroData> heroes = HeroDataLoader.LoadHeroesFromJson();

        // 캐릭터 데이터 추가
        infiniteScroll.InsertData(heroes.ToArray(), true);
    }
}