using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

public class FirebaseChatManager : MonoBehaviour
{
    public GameObject chatPanel;
    public GameObject textObject;
    public TMP_InputField chatInput;
    public Button sendButton;

    private FirebaseAuth _auth;
    private DatabaseReference _databaseRef;

    private List<Message> messageList = new List<Message>();

    private void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        sendButton.onClick.AddListener(SendMessageToDatabase);

        // Firebase Realtime Database에서 메시지 가져오기
        _databaseRef.Child("messages").ValueChanged += HandleMessageReceived;
    }

    private void HandleMessageReceived(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        foreach (Transform child in chatPanel.transform)
        {
            Destroy(child.gameObject);
        }

        messageList.Clear();

        foreach (DataSnapshot snapshot in args.Snapshot.Children)
        {
            Message message = JsonUtility.FromJson<Message>(snapshot.GetRawJsonValue());
            messageList.Add(message);
        }

        foreach (var message in messageList)
        {
            GameObject newText = Instantiate(textObject, chatPanel.transform);
            newText.GetComponent<TMP_Text>().text = message.displayName + ": " + message.text;
        }
    }

    public void SendMessageToDatabase()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            Message message = new Message(_auth.CurrentUser.DisplayName, chatInput.text);
            string json = JsonUtility.ToJson(message);
            _databaseRef.Child("messages").Push().SetRawJsonValueAsync(json);

            chatInput.text = "";
        }
    }
}
