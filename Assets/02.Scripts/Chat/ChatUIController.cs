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
    public GameObject chatMessagePrefab;

    private void Start()
    {
        chatScroll.PublicInitialize();
        sendButton.onClick.AddListener(OnSendButtonClicked);
    }

    private void OnSendButtonClicked()
    {
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            AddChatMessage("Me", message);
            chatInputField.text = string.Empty;
        }
    }

    public void AddChatMessage(string userName, string message)
    {
        var data = new ChatMessageData()
        {
            userName = userName,
            message = message,
            timestamp = DateTime.Now
        };
        chatScroll.InsertData(data);
    }
}