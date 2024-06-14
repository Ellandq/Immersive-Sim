using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemObject> allItems;

    public ItemObject GetItemByID(string id)
    {
        return allItems.Find(item => item.ID == id);
    }
}