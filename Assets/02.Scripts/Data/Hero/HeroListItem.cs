using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

public class HeroListItem : InfiniteScrollItem
{
    public Image portraitImage;
    public Image typeImage;
    public Image rankImage; // 캐릭터 등급에 따른 테두리 이미지
    public TMP_Text nameText;
    public TMP_Text levelText;
    
    public GameObject[] starIcons; // 별 갯수를 표시하는 아이콘 배열
    public Sprite[] rankSprites; // 랭크 스프라이트 배열
    public Sprite[] typeSprites; // 타입 스프라이트 배열

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        HeroData heroData = scrollData as HeroData;
        if (heroData == null) return;

        nameText.text = heroData.name;
        portraitImage.sprite = Resources.Load<Sprite>(heroData.portraitPath);
        typeImage.sprite = GetTypeSprite(heroData.type);
        levelText.text = $"Lv.{heroData.level}";

        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].SetActive(i < heroData.starLevel);
        }

        rankImage.sprite = GetRankSprite(heroData.rank);
    }

    private Sprite GetRankSprite(string grade)
    {
        return grade switch
        {
            "S" => rankSprites.Length > 0 ? rankSprites[0] : null,
            "A" => rankSprites.Length > 1 ? rankSprites[1] : null,
            "B" => rankSprites.Length > 2 ? rankSprites[2] : null,
            "C" => rankSprites.Length > 3 ? rankSprites[3] : null,
            "D" => rankSprites.Length > 4 ? rankSprites[4] : null,
            _ => null
        };
    }

    private Sprite GetTypeSprite(string type)
    {
        return type switch
        {
            "근거리형" => typeSprites.Length > 0 ? typeSprites[0] : null,
            "원거리형" => typeSprites.Length > 1 ? typeSprites[1] : null,
            "방어형" => typeSprites.Length > 2 ? typeSprites[2] : null,
            "만능형" => typeSprites.Length > 3 ? typeSprites[3] : null,
            _ => null
        };
    }
}