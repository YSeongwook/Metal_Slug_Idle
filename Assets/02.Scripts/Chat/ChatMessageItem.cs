using Gpm.Ui;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatMessageItem : InfiniteScrollItem
{
    public TMP_Text userNameText;
    public TMP_Text messageText;
    public Image userAvatarImage;
    public TMP_Text timestampText;
    public RectTransform rectTransform;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);

        ChatMessageData chatData = scrollData as ChatMessageData;
        if (chatData != null)
        {
            userNameText.text = chatData.userName;
            messageText.text = chatData.message;
            timestampText.text = chatData.timestamp.ToString("HH:mm");

            if (chatData.userAvatar != null)
            {
                userAvatarImage.sprite = chatData.userAvatar;
                userAvatarImage.gameObject.SetActive(true);
            }
            else
            {
                userAvatarImage.gameObject.SetActive(false);
            }

            // 메시지 텍스트가 길어질 경우 높이를 동적으로 설정
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            chatData.itemHeight = rectTransform.rect.height;
        }
    }
}