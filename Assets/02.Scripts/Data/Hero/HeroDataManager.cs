using System.Collections.Generic;
using EnumTypes;
using EventLibrary;

public class HeroDataManager : DataManager<HeroData>
{
    private List<HeroData> _allHeroes;
    private List<HeroData> _ownedHeroes;
    private bool isDescending = true;

    protected override void Awake()
    {
        base.Awake();
        EventManager<UIEvents>.StartListening(UIEvents.OnClickSortListAttackButton, ToggleSortOrder);
    }

    private void OnDestroy()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickSortListAttackButton, ToggleSortOrder);
    }

    protected override void Start()
    {
        LoadData();
    }

    public override void LoadData()
    {
        _allHeroes = DataLoader<HeroData>.LoadDataFromJson(fileName);
        _ownedHeroes = new List<HeroData>();

        foreach (HeroData hero in _allHeroes)
        {
            if (HeroCollectionManager.Instance.HasHero(hero.id))
            {
                _ownedHeroes.Add(hero);
            }
        }

        SortHeroesByAttack(_ownedHeroes, isDescending); // 기본적으로 내림차순 정렬

        infiniteScroll.ClearData();
        infiniteScroll.InsertData(_ownedHeroes.ToArray(), true);
        SetPaddingAndSpace();
        infiniteScroll.MoveToFirstData();
    }

    public void SortHeroesByAttack(List<HeroData> heroList, bool descending)
    {
        if (descending)
        {
            heroList.Sort((a, b) => b.attack.CompareTo(a.attack));
        }
        else
        {
            heroList.Sort((a, b) => a.attack.CompareTo(b.attack));
        }

        infiniteScroll.ClearData();
        infiniteScroll.InsertData(heroList.ToArray(), true);
        infiniteScroll.MoveToFirstData();
    }

    public List<HeroData> GetOwnedHeroes()
    {
        List<HeroData> allHeroes = DataLoader<HeroData>.LoadDataFromJson(fileName);
        List<HeroData> ownedHeroes = new List<HeroData>();

        foreach (HeroData hero in allHeroes)
        {
            if (HeroCollectionManager.Instance.HasHero(hero.id))
            {
                ownedHeroes.Add(hero);
            }
        }

        return ownedHeroes;
    }

    private void ToggleSortOrder()
    {
        isDescending = !isDescending;
        SortHeroesByAttack(_ownedHeroes, isDescending);
    }
}
