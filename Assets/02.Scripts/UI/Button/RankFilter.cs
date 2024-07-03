using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankFilter : MonoBehaviour
{
    [SerializeField] private TMP_Text rankButtonText; // 텍스트를 변경할 버튼
    public string heroRank; // 버튼에 설정된 랭크
    
    private Button _rankButton;
    private GameObject _parent;

    private void Awake()
    {
        _rankButton = GetComponent<Button>();
        _parent = gameObject.transform.parent.gameObject;
    }

    private void Start()
    {
        _rankButton.onClick.AddListener(OnRankButtonClick);
    }

    private void OnDestroy()
    {
        _rankButton.onClick.RemoveListener(OnRankButtonClick);
    }

    private void OnRankButtonClick()
    {
        HeroDataManager.Instance.FilterHeroesByRank(heroRank);
        rankButtonText.text = heroRank == "전체" ? heroRank : $"{heroRank} 랭크"; // 버튼의 텍스트 변경

        UIManager.Instance.ToggleUIWithoutMainClose(_parent);
    }
}