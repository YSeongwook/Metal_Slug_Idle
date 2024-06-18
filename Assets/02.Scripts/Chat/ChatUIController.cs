using System;
using EnumTypes;
using Firebase.Database;
using Firebase.Auth;
using Gpm.Ui;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using EventLibrary;

namespace Chat
{
    public class ChatUIController : MonoBehaviour
    {
        public InfiniteScroll chatScroll;
        public TMP_InputField chatInputField;
        public Button sendButton;

        private Logger logger;
        private ScrollRect _scrollRect;
        private FirebaseDataManager _firebaseDataManager;
        private FirebaseUser _currentUser; // 현재 로그인한 사용자
        private DatabaseReference _messagesReference; // 메시지 참조 캐시
        private bool _isDatabaseInitialized;
        private bool _isUserSignIn;

        private void Awake()
        {
            // Firebase 초기화 완료 이벤트 리스너 등록
            EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseDatabaseInitialized,
                OnFirebaseDatabaseInitialized);
            // Firebase 로그인 완료 이벤트 리스너 등록
            EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
        }

        private void Start()
        {
            _scrollRect = chatScroll.GetComponent<ScrollRect>();
            _firebaseDataManager = FirebaseDataManager.Instance;

            sendButton.onClick.AddListener(OnSendButtonClicked);

            logger = Logger.Instance;
        }

        private void OnDestroy()
        {
            // 이벤트 리스너 해제
            EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseDatabaseInitialized,
                OnFirebaseDatabaseInitialized);
            EventManager<FirebaseEvents>.StopListening(FirebaseEvents.FirebaseSignIn, OnFirebaseSignIn);
        }

        private void OnFirebaseDatabaseInitialized()
        {
            _isDatabaseInitialized = true;
            CheckInitializationStatus();
        }

        private void OnFirebaseSignIn()
        {
            _currentUser = AuthManager.Instance.GetCurrentUser();
            _isUserSignIn = true;
            CheckInitializationStatus();
        }

        private void CheckInitializationStatus()
        {
            if (_isDatabaseInitialized && _isUserSignIn)
            {
                // messagesReference 캐싱
                _messagesReference = _firebaseDataManager.DatabaseReference.Child("messages");
                logger.LogError($"{_messagesReference}");

                // Firebase 초기화 완료 후 메시지 로드
                // LoadChatMessages();
            }
        }

        private void OnSendButtonClicked()
        {
            logger.Log("버튼 클릭함");
            string message = chatInputField.text;
            if (!string.IsNullOrEmpty(message))
            {
                AddChatMessage(_currentUser.DisplayName, message, DateTime.Now, null); // 현재 사용자 DisplayName을 사용
                chatInputField.text = string.Empty;
                ScrollToBottom();
            }
            else
            {
                message = "InputField가 null";
                AddChatMessage(_currentUser.DisplayName, message, DateTime.Now, null);
            }
        }

        private void AddChatMessage(string userName, string message, DateTime timestamp, Sprite userAvatar)
        {
            var data = new ChatMessageData()
            {
                userName = userName,
                message = message,
                Timestamp = timestamp, // Unix 타임스탬프로 변환하여 저장
                userAvatar = userAvatar
            };

            // Firebase에 데이터 저장
            string key = _messagesReference.Push().Key;
            _messagesReference.Child(key).SetRawJsonValueAsync(JsonUtility.ToJson(data)).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    logger.Log("메시지 저장이 취소되었습니다.");
                    return;
                }

                if (task.IsFaulted)
                {
                    logger.LogError("메시지 저장 중 오류 발생: " + task.Exception);
                    return;
                }
            });
            
            chatScroll.InsertData(data);
        }

        private void LoadChatMessages()
        {
            try
            {
                _messagesReference.GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        logger.Log("채팅 메시지 불러오기가 취소되었습니다.");
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        logger.LogError("채팅 메시지 불러오기 중 오류 발생: " + task.Exception);
                        return;
                    }

                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot messageSnapshot in snapshot.Children)
                    {
                        try
                        {
                            var data = JsonUtility.FromJson<ChatMessageData>(messageSnapshot.GetRawJsonValue());
                            chatScroll.InsertData(data);
                        }
                        catch (Exception e)
                        {
                            logger.LogError("메시지 처리 중 오류 발생: " + e.Message);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                logger.LogError("채팅 메시지 로드 중 예외 발생: " + e.Message);
            }
        }

        private void ScrollToBottom()
        {
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 0f; // 0이면 가장 아래로 스크롤
        }
    }
}