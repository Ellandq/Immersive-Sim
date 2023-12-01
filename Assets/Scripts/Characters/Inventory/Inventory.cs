using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary<ItemType, ItemCollection> items;

    private long gold;

    private void Awake ()
    {
        items = new Dictionary<ItemType, ItemCollection>();

        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            items[type] = new ItemCollection(type);
        }
    }

    public void AddItem (Item item)
    {
        items[item.GetItemType()].AddItem(item);
    }

    public void AddItems (List<Item> items)
    {
        foreach (Item item in items) AddItem(item);
    }

    public bool RemoveItem (Item item)
    {
        try {
            items[item.GetItemType()].RemoveItem(item);
            return true;
        } catch (ItemNegativeCountException e){
            Debug.LogError(e.Message);
            return false;
        }
    }

}
