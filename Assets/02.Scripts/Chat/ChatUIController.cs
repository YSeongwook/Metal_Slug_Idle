using System;
using Gpm.Ui;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatUIController : MonoBehaviour
{
    public InfiniteScroll scrollViewChat;
    public TMP_InputField inputFieldChat;
    public Button btnSend;
    public GameObject chatMessagePrefab;

    private void Start()
    {
        scrollViewChat.PublicInitialize();
        btnSend.onClick.AddListener(OnSendButtonClicked);
    }

    private void OnSendButtonClicked()
    {
        string message = inputFieldChat.text;
        if (!string.IsNullOrEmpty(message))
        {
            AddChatMessage("Me", message);
            inputFieldChat.text = string.Empty;
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
        scrollViewChat.InsertData(data);
    }
}