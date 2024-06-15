using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemDatabaseEntity> allItems;
}

[System.Serializable]
public class ItemDatabaseEntity
{
    public ItemObject ItemObject;
    public ItemModifier ItemModifier;
    public string Name;
    public string Path;

    public ItemDatabaseEntity(string name, string path)
    {
        Name = name;
        Path = path;
    }

    public override string ToString()
    {
        return Name + ", " + Path;
    }
}