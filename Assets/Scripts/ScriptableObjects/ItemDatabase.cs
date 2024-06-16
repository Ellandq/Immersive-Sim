using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemDatabaseEntity> allItems;

    public ItemDatabaseEntity GetByID(string ID)
    {
        return allItems.FirstOrDefault(item => item.ItemObject.ID == ID);
    }
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
        return Path + "\\" + Name;
    }
}