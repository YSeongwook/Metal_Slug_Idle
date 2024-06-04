using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference mReference;

    void Start()
    {
        mReference = FirebaseDatabase.DefaultInstance.RootReference;

        ReadUserData();
    }

    private void ReadUserData()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do Something with Snapshot...
                for (int i = 0; i < snapshot.ChildrenCount; i++)
                {
                    Debug.Log(snapshot.Child(i.ToString()).Child("username").Value);
                }
            }
        });
    }

    private void WriteUserData(string userId, string userName)
    {
        mReference.Child("users").Child(userId).Child("userName").SetValueAsync(userName);
    }
}