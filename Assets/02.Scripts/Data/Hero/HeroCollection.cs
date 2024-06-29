// HeroCollection.cs
using System;
using System.Collections.Generic;

[Serializable]
public class HeroCollection
{
    public List<HeroCollectionItem> heroes = new List<HeroCollectionItem>();
}

[Serializable]
public class HeroCollectionItem
{
    public int id;
    public bool owned;
}