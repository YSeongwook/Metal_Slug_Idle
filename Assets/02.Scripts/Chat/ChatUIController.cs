using System;
using Gpm.Ui;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatUIController : MonoBehaviour
{
    public InfiniteScroll chatScroll;
    public TMP_InputField chatInputField;
    public Button sendButton;
    
    private ScrollRect scrollRect;

    private void Start()
    {
        scrollRect = chatScroll.GetComponent<ScrollRect>();
        chatScroll.onChangeValue.AddListener(OnChatScrollValueChanged);
        sendButton.onClick.AddListener(OnSendButtonClicked);
    }

    private void OnSendButtonClicked()
    {
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            AddChatMessage("Me", message, DateTime.Now, null); // Avatar는 null로 설정
            chatInputField.text = string.Empty;
            ScrollToBottom();
        }
    }

    public void AddChatMessage(string userName, string message, DateTime timestamp, Sprite userAvatar)
    {
        var data = new ChatMessageData()
        {
            userName = userName,
            message = message,
            timestamp = timestamp,
            userAvatar = userAvatar
        };
        chatScroll.InsertData(data);
    }

    private void OnChatScrollValueChanged(int firstIndex, int lastIndex, bool isStart, bool isEnd)
    {
        // 스크롤 변경 시 호출되는 메서드
        // 필요에 따라 데이터를 더 로드하거나 다른 작업을 수행할 수 있습니다.
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // 0이면 가장 아래로 스크롤
    }
}