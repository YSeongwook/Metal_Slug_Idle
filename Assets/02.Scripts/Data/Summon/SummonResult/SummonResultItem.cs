using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

public class SummonResultItem : InfiniteScrollItem
{
    public Image portrait;
    public TMP_Text count;
    public int id;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        SummonResultData summonResultData = scrollData as SummonResultData;
        if (summonResultData == null) return;

        portrait.sprite = Resources.Load<Sprite>(summonResultData.portraitPath);
        count.text = $"{summonResultData.count}";
    }
}