using Gpm.Ui;
using TMPro;
using UnityEngine.UI;

public class ChatMessageItem : InfiniteScrollItem
{
    public Image playerPortrait;  // 플레이어 초상화
    public TMP_Text playerNameText; // 플레이어 이름 텍스트
    public TMP_Text messageText;   // 채팅 내용 텍스트
    public TMP_Text timestampText; // 타임스탬프 텍스트
    public Image chatBackground;   // 말풍선 배경 이미지

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);

        if (scrollData is ChatMessageData data)
        {
            playerPortrait.sprite = data.userAvatar;
            playerNameText.text = data.userName;
            messageText.text = data.message;
            timestampText.text = data.timestamp.ToString("g"); // 예: "오후 3:24"
        }
    }
}