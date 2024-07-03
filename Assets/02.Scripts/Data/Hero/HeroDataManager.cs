using System.Collections.Generic;
using System.Linq;
using EnumTypes;
using EventLibrary;
using Gpm.Ui;
using UnityEngine;

public class HeroDataManager : Singleton<HeroDataManager>
{
    public InfiniteScroll infiniteScroll; // InfiniteScroll 컴포넌트 참조
    public Vector2 padding; // 패딩 값
    public Vector2 space; // 스페이스 값
    public string fileName; // JSON 파일 이름
    
    private List<HeroData> _allHeroes; // 모든 영웅 데이터를 저장하는 리스트
    private List<HeroData> _ownedHeroes; // 소유한 영웅 데이터를 저장하는 리스트
    private bool _isDescending = true; // 정렬 순서를 저장하는 변수 (기본값: 내림차순)

    private string currentTypeFilter = "전체"; // 현재 타입 필터 상태
    private string currentRankFilter = "전체"; // 현재 랭크 필터 상태

    protected override void Awake()
    {
        base.Awake();
        // 정렬 버튼 클릭 이벤트 리스너 추가
        EventManager<UIEvents>.StartListening(UIEvents.OnClickSortListAttackButton, ToggleSortOrder);
    }

    private void OnDestroy()
    {
        // 정렬 버튼 클릭 이벤트 리스너 제거
        EventManager<UIEvents>.StopListening(UIEvents.OnClickSortListAttackButton, ToggleSortOrder);
    }

    private void Start()
    {
        LoadData(); // 데이터 로드
    }

    private void LoadData()
    {
        _allHeroes = DataLoader<HeroData>.LoadDataFromJson(fileName);
        _ownedHeroes = new List<HeroData>();

        // 소유한 영웅 리스트 필터링
        foreach (HeroData hero in _allHeroes)
        {
            if (HeroCollectionManager.Instance.HasHero(hero.id))
            {
                _ownedHeroes.Add(hero);
            }
        }

        ApplyFilters(); // 필터 및 정렬 적용
        SetPaddingAndSpace(); // 패딩 및 스페이스 설정
        infiniteScroll.MoveToFirstData();
    }

    // 영웅 리스트를 공격력 기준으로 정렬하는 메서드
    private void SortHeroesByAttack(List<HeroData> heroList, bool descending)
    {
        if (descending)
        {
            heroList.Sort((a, b) => b.attack.CompareTo(a.attack));
        }
        else
        {
            heroList.Sort((a, b) => a.attack.CompareTo(b.attack));
        }
    }
    
    // 패딩과 스페이스 설정 메서드
    private void SetPaddingAndSpace()
    {
        infiniteScroll.SetPadding(padding);
        infiniteScroll.SetSpace(space);
    }

    // 소유한 영웅 리스트를 반환하는 메서드
    public List<HeroData> GetOwnedHeroes()
    {
        return _ownedHeroes;
    }

    // 정렬 순서를 토글하는 메서드
    private void ToggleSortOrder()
    {
        _isDescending = !_isDescending;
        ApplyFilters(); // 정렬 상태 변경 후 필터 및 정렬 적용
    }

    // 타입 필터를 적용하는 메서드
    public void FilterHeroesByType(string type)
    {
        currentTypeFilter = type;
        ApplyFilters(); // 타입 필터 적용 후 필터 및 정렬 적용
    }

    // 랭크 필터를 적용하는 메서드
    public void FilterHeroesByRank(string rank)
    {
        currentRankFilter = rank;
        ApplyFilters(); // 랭크 필터 적용 후 필터 및 정렬 적용
    }

    // 현재 적용된 필터들을 사용하여 영웅 리스트를 필터링하고 정렬하는 메서드
    private void ApplyFilters()
    {
        var filteredHeroes = _ownedHeroes.Where(hero =>
            (currentTypeFilter == "전체" || hero.type == currentTypeFilter) &&
            (currentRankFilter == "전체" || hero.rank == currentRankFilter)).ToList();

        // 배치된 영웅과 배치되지 않은 영웅을 나눔
        var assignedHeroes = filteredHeroes.Where(hero => HeroCollectionManager.Instance.IsHeroAssigned(hero.id)).ToList();
        var unassignedHeroes = filteredHeroes.Where(hero => !HeroCollectionManager.Instance.IsHeroAssigned(hero.id)).ToList();

        // 각각 정렬
        SortHeroesByAttack(assignedHeroes, _isDescending);
        SortHeroesByAttack(unassignedHeroes, _isDescending);

        // 두 리스트를 합침
        assignedHeroes.AddRange(unassignedHeroes);
        UpdateHeroList(assignedHeroes); // 필터링된 영웅 리스트를 UI에 업데이트
    }

    // 필터링된 영웅 리스트를 UI에 업데이트하는 메서드
    private void UpdateHeroList(List<HeroData> heroList)
    {
        infiniteScroll.ClearData();
        infiniteScroll.InsertData(heroList.ToArray(), true);
        infiniteScroll.MoveToFirstData();
    }
}
