using System;

[Serializable]
public class User
{
    public string userId;
    public string displayName;
    public int level;
    public string items;

    public User(string userId, string displayName, int level, string items)
    {
        this.userId = userId;
        this.displayName = displayName;
        this.level = level;
        this.items = items;
    }
}