using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

public class SummonResultItem : InfiniteScrollItem
{
    public Image portrait;
    public Image rankImage; // 캐릭터 등급에 따른 이미지
    public TMP_Text count;
    public Sprite[] rankSprites; // 랭크 스프라이트 배열

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        SummonResultData summonResultData = scrollData as SummonResultData;
        if (summonResultData == null) return;

        // 초상화 이미지 설정
        string portraitPath = summonResultData.portraitPath;
        Sprite portraitSprite = Resources.Load<Sprite>(portraitPath);
        if (portraitSprite != null)
        {
            portrait.sprite = portraitSprite;
        }

        // 랭크 이미지 설정
        rankImage.sprite = GetRankSprite(summonResultData.rank);

        // 소환된 개수 텍스트 설정
        count.text = $"{summonResultData.count}";
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
}