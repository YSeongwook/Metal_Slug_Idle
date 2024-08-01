using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class DatabaseManager : Singleton<DatabaseManager>
{
    private DatabaseReference _databaseRef;
    private FirebaseAuth _auth;
    private FirebaseUser currentUser;

    protected override void Awake()
    {
        base.Awake();
        _auth = FirebaseAuth.DefaultInstance;
        _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public FirebaseUser GetCurrentUser()
    {
        return _auth.CurrentUser;
    }

    public async Task SaveUserData(UserData userData)
    {
        var updates = new Dictionary<string, object>
        {
            { "displayName", userData.displayName },
            { "level", userData.level },
            { "items", userData.items },
            { "lastUpdated", userData.lastUpdated }
        };

        await _databaseRef.Child("users").Child(userData.userId).UpdateChildrenAsync(updates).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                DebugLogger.LogError("유저 데이터 저장이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                DebugLogger.LogError("유저 데이터 저장 중 오류 발생: " + task.Exception);
                return;
            }

            DebugLogger.Log("유저 데이터 저장 성공.");
        });
    }

    public async Task<UserData> LoadUserData(string userId)
    {
        var userDataSnapshot = await _databaseRef.Child("users").Child(userId).GetValueAsync();

        if (userDataSnapshot.Exists)
        {
            return new UserData(
                userId,
                userDataSnapshot.Child("displayName").Value.ToString(),
                int.Parse(userDataSnapshot.Child("level").Value.ToString()),
                userDataSnapshot.Child("items").Value.ToString(),
                long.Parse(userDataSnapshot.Child("lastUpdated").Value.ToString())
            );
        }
        else
        {
            DebugLogger.LogWarning("유저 데이터가 존재하지 않습니다.");
            return null;
        }
    }

    public async Task SaveHeroCollection(UserHeroCollection userHeroCollection)
    {
        var updates = new Dictionary<string, object>
        {
            { "heroCollection", userHeroCollection.heroCollectionBase64 }
        };

        await _databaseRef.Child("user_HeroCollection").Child(userHeroCollection.userId).UpdateChildrenAsync(updates).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                DebugLogger.LogError("유저 히어로 컬렉션 저장이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                DebugLogger.LogError("유저 히어로 컬렉션 저장 중 오류 발생: " + task.Exception);
                return;
            }

            DebugLogger.Log("유저 히어로 컬렉션 저장 성공.");
        });
    }

    public async Task<UserHeroCollection> LoadHeroCollection(string userId)
    {
        var heroDataSnapshot = await _databaseRef.Child("user_HeroCollection").Child(userId).GetValueAsync();

        if (heroDataSnapshot.Exists)
        {
            string base64HeroCollection = heroDataSnapshot.Child("heroCollection").Value.ToString();
            return new UserHeroCollection(userId, base64HeroCollection);
        }
        else
        {
            DebugLogger.LogWarning("유저 히어로 컬렉션 데이터가 존재하지 않습니다.");
            return null;
        }
    }

    public async Task UpdateHeroCollection(string userId, int[] heroIds)
    {
        var heroDataSnapshot = await _databaseRef.Child("user_HeroCollection").Child(userId).GetValueAsync();

        if (heroDataSnapshot.Exists)
        {
            string base64HeroCollection = heroDataSnapshot.Child("heroCollection").Value.ToString();
            HeroCollectionManager.Instance.FromBase64(base64HeroCollection);

            foreach (int heroId in heroIds)
            {
                HeroCollectionManager.Instance.AddHero(heroId);
            }

            var userHeroCollection = new UserHeroCollection(userId, HeroCollectionManager.Instance.ToBase64());
            await SaveHeroCollection(userHeroCollection);
        }
        else
        {
            DebugLogger.LogWarning("유저 히어로 컬렉션 데이터가 존재하지 않습니다.");
        }
    }

    public async Task DeleteAllData()
    {
        await _databaseRef.RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                DebugLogger.LogError("모든 데이터 삭제가 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                DebugLogger.LogError("모든 데이터 삭제 중 오류 발생: " + task.Exception);
                return;
            }

            DebugLogger.Log("모든 데이터 삭제 성공.");
        });
    }

    public async void LoadHeroDataFromFirebase(Action<HeroDataWrapper> callback)
    {
        if (_databaseRef == null || GetCurrentUser() == null)
        {
            DebugLogger.LogError("Firebase Database or User not initialized.");
            return;
        }

        var heroDataSnapshot = await _databaseRef.Child("HeroData").GetValueAsync();

        if (heroDataSnapshot.Exists)
        {
            var heroDataJson = heroDataSnapshot.GetRawJsonValue();
            var heroDataWrapper = JsonUtility.FromJson<HeroDataWrapper>(heroDataJson);
            callback(heroDataWrapper);
        }
        else
        {
            DebugLogger.Log("HeroData not found in Firebase Realtime Database.");
        }
    }
}
