using System;
using Gpm.Ui;
using UnityEngine;

public class ChatMessageData : InfiniteScrollData
{
    public string userName;
    public string message;
    public DateTime timestamp;
    public Sprite userAvatar;
    public float itemHeight; // 메시지의 높이를 저장할 변수 추가
}