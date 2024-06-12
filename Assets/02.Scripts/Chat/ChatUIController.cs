using System;
using EnumTypes;
using Firebase.Database;
using Firebase.Auth;
using Gpm.Ui;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using EventLibrary;

public class ChatUIController : MonoBehaviour
{
    public InfiniteScroll chatScroll;
    public TMP_InputField chatInputField;
    public Button sendButton;
    
    private ScrollRect scrollRect;
    private FirebaseDataManager firebaseDataManager;
    private Logger logger;
    private FirebaseUser currentUser; // 현재 로그인한 사용자

    private void Start()
    {
        scrollRect = chatScroll.GetComponent<ScrollRect>();
        firebaseDataManager = FirebaseDataManager.Instance;

        chatScroll.onChangeValue.AddListener(OnChatScrollValueChanged);
        sendButton.onClick.AddListener(OnSendButtonClicked);
        
        logger = Logger.Instance;

        // Firebase 초기화 완료 이벤트 리스너 등록
        EventManager<FirebaseEvents>.StartListening(FirebaseEvents.FirebaseInitialized, OnFirebaseInitialized);
        // Firebase 로그인 완료 이벤트 리스너 등록
        EventManager<FirebaseEvents>.StartListening<FirebaseUser>(FirebaseEvents.FirebaseLoggedIn, OnFirebaseLoggedIn);
    }

    private void OnFirebaseInitialized()
    {
        // Firebase 초기화 완료 후 메시지 로드
        LoadChatMessages();
    }

    private void OnFirebaseLoggedIn(FirebaseUser user)
    {
        // Firebase 로그인 완료 후 사용자 정보 저장
        currentUser = user;
    }

    private void OnSendButtonClicked()
    {
        logger.Log("버튼 클릭함");
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            AddChatMessage(currentUser.UserId, message, DateTime.Now, null); // 현재 사용자 ID를 사용
            chatInputField.text = string.Empty;
            ScrollToBottom();
        }
        else
        {
            message = "InputField가 null";
            AddChatMessage(currentUser.UserId, message, DateTime.Now, null);
        }
    }

    public void AddChatMessage(string userName, string message, DateTime timestamp, Sprite userAvatar)
    {
        logger.Log("AddChatMessage");
        var data = new ChatMessageData()
        {
            userName = userName,
            message = message,
            timestamp = timestamp,
            userAvatar = userAvatar
        };
        
        // Firebase에 데이터 저장
        string key = firebaseDataManager.DatabaseReference.Child("messages").Push().Key;
        firebaseDataManager.DatabaseReference.Child("messages").Child(key).SetRawJsonValueAsync(JsonUtility.ToJson(data));
        
        chatScroll.InsertData(data);
        // 새로운 데이터를 추가한 후 높이를 업데이트
        UpdateItemHeight(data);
    }

    private void LoadChatMessages()
    {
        firebaseDataManager.DatabaseReference.Child("messages").GetValueAsync().ContinueWith(task =>
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
                var data = JsonUtility.FromJson<ChatMessageData>(messageSnapshot.GetRawJsonValue());
                chatScroll.InsertData(data);
                UpdateItemHeight(data);
            }
            ScrollToBottom();
        });
    }

    private void UpdateItemHeight(ChatMessageData data)
    {
        chatScroll.UpdateData(data);
        chatScroll.UpdateAllData(true);
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
