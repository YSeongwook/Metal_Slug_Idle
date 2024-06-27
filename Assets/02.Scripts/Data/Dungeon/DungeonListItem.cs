using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

public class DungeonListItem : InfiniteScrollItem
{
    public Image background;
    public Image keyIcon;
    public TMP_Text nameText;
    public TMP_Text explainText;
    public TMP_Text keyRemainText;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        DungeonData dungeonData = scrollData as DungeonData;
        if (dungeonData == null) return;

        nameText.text = dungeonData.name;
        background.sprite = Resources.Load<Sprite>(dungeonData.backgroundPath);
        keyIcon.sprite = Resources.Load<Sprite>(dungeonData.keyIconPath);
        explainText.text = $"{dungeonData.explain}";
        keyRemainText.text = $"{dungeonData.keyRemain} / 3";
    }
}