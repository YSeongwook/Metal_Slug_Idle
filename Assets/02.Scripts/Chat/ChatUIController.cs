using System;
using EnumTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventLibrary;
using Firebase.Auth;
using Gpm.Ui;
using Newtonsoft.Json;

namespace Chat
{
    public class ChatUIController : MonoBehaviour
    {
        public InfiniteScroll chatScroll;
        public TMP_InputField chatInputField;
        public Button sendButton;

        private Logger logger;
        private ScrollRect _scrollRect;
        private FirebaseUser _currentUser; // 현재 로그인한 사용자
        private bool _isUserSignIn;
        private TcpClientManager _tcpClientManager;

        public TcpChatServer tcpChatServer;

        [SerializeField]
        private string serverIp = "127.0.0.1"; // 기본값을 로컬호스트로 설정
        [SerializeField]
        private int port = 8080;

        private void Awake()
        {
            EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
        }

        private void OnEnable()
        {
            _scrollRect = chatScroll.GetComponent<ScrollRect>();
            sendButton.onClick.AddListener(OnSendButtonClicked);
            logger = Logger.Instance;

            tcpChatServer = FindObjectOfType<TcpChatServer>();
            if (tcpChatServer == null)
            {
                Debug.LogError("TcpChatServer를 찾을 수 없습니다.");
                return;
            }

            _tcpClientManager = new TcpClientManager(serverIp, port);
            _tcpClientManager.OnMessageReceived += HandleMessageReceived;
            Debug.Log("Chat UI 활성화됨 - 서버에 연결");
        }

        private void OnDisable()
        {
            _tcpClientManager?.Disconnect();
            Debug.Log("Chat UI 비활성화됨 - 서버 연결 해제");
        }

        private void OnDestroy()
        {
            EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
        }

        private void OnFirebaseSignIn()
        {
            _currentUser = AuthManager.Instance.GetCurrentUser();
            _isUserSignIn = true;
            logger.Log($"사용자 {_currentUser.DisplayName}로 로그인됨");
        }

        private void OnSendButtonClicked()
        {
            logger.Log("버튼 클릭함");
            string message = chatInputField.text;
            if (!string.IsNullOrEmpty(message))
            {
                var chatMessage = new ChatMessageData
                {
                    userName = _currentUser?.DisplayName,
                    message = message,
                    Timestamp = DateTime.Now,
                    userAvatar = null // 필요에 따라 아바타 설정
                };

                string jsonMessage = JsonConvert.SerializeObject(chatMessage);
                _tcpClientManager.SendMessageToServer(jsonMessage);
                logger.Log("보낸 메시지: " + jsonMessage);
                chatInputField.text = string.Empty;
                UpdateChatDisplay(chatMessage);
                ScrollToBottom();
            }
        }

        private void HandleMessageReceived(string jsonMessage)
        {
            var chatMessage = JsonConvert.DeserializeObject<ChatMessageData>(jsonMessage);
            logger.Log("받은 메시지: " + jsonMessage);
            UnityMainThreadDispatcher.Enqueue(() => UpdateChatDisplay(chatMessage));
        }

        private void UpdateChatDisplay(ChatMessageData chatMessage)
        {
            chatScroll.InsertData(chatMessage);
        }

        private void ScrollToBottom()
        {
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 0f; // 0이면 가장 아래로 스크롤
        }
    }
}
