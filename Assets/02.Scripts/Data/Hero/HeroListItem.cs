using Gpm.Ui;
using UnityEngine;
using UnityEngine.UI;

public class HeroListItem : InfiniteScrollItem
{
    public Text nameText;
    public Image portraitImage;
    public GameObject[] starIcons; // 별 갯수를 표시하는 아이콘 배열
    public Text typeText;
    public Text levelText;
    public Image gradeFrame; // 캐릭터 등급에 따른 테두리 이미지

    public Sprite frameD;
    public Sprite frameC;
    public Sprite frameB;
    public Sprite frameA;
    public Sprite frameS;
    public Sprite frameSSS; // SSS 등급 추가

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        HeroData heroData = scrollData as HeroData;
        if (heroData == null) return;

        nameText.text = heroData.name;
        portraitImage.sprite = Resources.Load<Sprite>(heroData.portraitPath);
        typeText.text = heroData.type;
        levelText.text = $"Lv. {heroData.level}";

        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].SetActive(i < heroData.starLevel);
        }

        switch (heroData.grade)
        {
            case "D":
                gradeFrame.sprite = frameD;
                break;
            case "C":
                gradeFrame.sprite = frameC;
                break;
            case "B":
                gradeFrame.sprite = frameB;
                break;
            case "A":
                gradeFrame.sprite = frameA;
                break;
            case "S":
                gradeFrame.sprite = frameS;
                break;
            case "SSS":
                gradeFrame.sprite = frameSSS;
                break;
            default:
                gradeFrame.sprite = null;
                break;
        }
    }
}