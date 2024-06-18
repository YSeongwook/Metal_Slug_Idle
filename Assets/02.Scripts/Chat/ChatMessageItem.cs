using Cysharp.Threading.Tasks;
using Gpm.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageItem : InfiniteScrollItem
{
    public TMP_Text userNameText;
    public TMP_Text messageText;
    public Image userAvatarImage;
    public TMP_Text timestampText;

    public ContentSizeFitter contentSizeFitter;
    
    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);

        ChatMessageData chatData = scrollData as ChatMessageData;
        
        if (chatData == null) return;
        
        userNameText.text = chatData.userName;
        messageText.text = chatData.message;
        timestampText.text = chatData.Timestamp.ToString("HH:mm"); // Unix 타임스탬프를 DateTime으로 변환하여 표시

        if (chatData.userAvatar != null)
        {
            userAvatarImage.sprite = chatData.userAvatar;
            userAvatarImage.gameObject.SetActive(true);
        }
        else
        {
            userAvatarImage.gameObject.SetActive(false);
        }
        
        // 레이아웃 업데이트 강제 수행
        // Canvas.ForceUpdateCanvases();
        waitTask();
    }

    async UniTask waitTask()
    {
        await UniTask.NextFrame();
        
        SetSize(GetSecondChildHeight());
    }
    
    private float GetSecondChildHeight()
    {
        Canvas.ForceUpdateCanvases();
        RectTransform secondChildRect = transform.GetChild(1) as RectTransform;
        if (secondChildRect != null)
        {
            return secondChildRect.rect.height + 20f;
        }

        return 0;
    }
}