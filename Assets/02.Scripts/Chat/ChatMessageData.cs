using System;
using Gpm.Ui;
using UnityEngine;

public class ChatMessageData : InfiniteScrollData
{
    public string userName;
    public string message;
    public DateTime timestamp;
    public Sprite userAvatar;
}