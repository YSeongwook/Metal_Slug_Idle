using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

public class SummonListItem : InfiniteScrollItem
{
    public Image background;
    public Image banner;
    public TMP_Text nameText;
    public TMP_Text explainText;
    public TMP_Text endTime;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        SummonData summonData = scrollData as SummonData;
        if (summonData == null) return;

        nameText.text = $"{summonData.name} 픽업 소환";
        background.sprite = Resources.Load<Sprite>(summonData.backgroundPath);
        banner.sprite = Resources.Load<Sprite>(summonData.bannerPath);
        explainText.text = $"{summonData.explain}";
        endTime.text = $"{summonData.endTime / 3}";
    }
}