using Gpm.Ui;
using UnityEngine;
using TMPro;

public class ChatMessageItem : InfiniteScrollItem
{
    public TMP_Text userNameText;
    public TMP_Text messageText;
    public TMP_Text timestampText;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);

        ChatMessageData chatData = (ChatMessageData)scrollData;
        userNameText.text = chatData.userName;
        messageText.text = chatData.message;
        timestampText.text = chatData.timestamp.ToString("HH:mm");
    }
}