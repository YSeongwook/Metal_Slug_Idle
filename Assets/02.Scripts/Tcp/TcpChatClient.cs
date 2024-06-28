using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class ChatMessage
{
    public string UserName { get; set; }
    public string AvatarKey { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}

public class TcpChatClient : MonoBehaviour
{
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private byte[] _buffer = new byte[1024];

    public TMP_InputField inputFieldChat;
    public TextMeshProUGUI chatDisplay;
    public string userName;
    public string avatarKey;

    private void Start()
    {
        _tcpClient = new TcpClient("127.0.0.1", 8080);
        _networkStream = _tcpClient.GetStream();
        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();
    }

    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(inputFieldChat.text))
        {
            var chatMessage = new ChatMessage
            {
                UserName = userName,
                AvatarKey = avatarKey,
                Message = inputFieldChat.text
            };

            string jsonMessage = JsonConvert.SerializeObject(chatMessage);
            byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            _networkStream.Write(messageBytes, 0, messageBytes.Length);
            Debug.Log("보낸 메시지: " + jsonMessage);
            inputFieldChat.text = string.Empty;
        }
    }

    private void ReceiveMessages()
    {
        int bytesRead;

        try
        {
            while ((bytesRead = _networkStream.Read(_buffer, 0, _buffer.Length)) != 0)
            {
                string jsonMessage = Encoding.UTF8.GetString(_buffer, 0, bytesRead);
                Debug.Log("받은 메시지: " + jsonMessage);
                var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(jsonMessage);
                UpdateChatDisplay(chatMessage);
            }
        }
        catch (Exception e)
        {
            Debug.Log("서버 연결 종료: " + e.Message);
        }
    }
    
    private void UpdateChatDisplay(ChatMessage chatMessage)
    {
        if (chatDisplay != null)
        {
            string formattedMessage = $"{chatMessage.UserName} ({chatMessage.Timestamp.ToShortTimeString()}): {chatMessage.Message}";
            chatDisplay.text += formattedMessage + "\n";
            // TODO: 초상화를 표시하는 로직 추가
            // Sprite avatar = LoadAvatar(chatMessage.AvatarKey);
            // DisplayAvatar(avatar);
        }
    }

    private void OnDestroy()
    {
        _networkStream.Close();
        _tcpClient.Close();
    }
}