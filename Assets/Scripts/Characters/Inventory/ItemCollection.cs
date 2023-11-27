using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ItemCollection 
{
    private ItemType collectionType;
    private List<Item> items;

    public ItemCollection (ItemType collectionType)
    {
        this.collectionType = collectionType;
        items = new List<Item>();
    }

    public void AddItem (Item item)
    {
        if (item != collectionType) throw new ItemTypeException ($"Cannot insert an item of type {item.ItemData.Type} into a collection of type {collectionType}");

        if (item.ItemData.IsStackable && Contains(item)) 
        { 
            items[GetSameItemIndex(item)] += item;
        }
        else
        {
            items.Add(item);
        }
    }

    public void RemoveItem (Item item)
    {
        if (item != collectionType) throw new ItemTypeException ($"Cannot insert an item of type {item.ItemData.Type} into a collection of type {collectionType}");

        if (item.ItemData.IsStackable && Contains(item)) 
        { 
            int index = GetSameItemIndex(item);
            items[index] -= item;
            if (items[index].IsEmpty()) items.RemoveAt(index);
        }
    }

    public void SortCollection ()
    {
        foreach (Item item in items)
        {
            // TODO
        }
    }

    public int GetSameItemIndex (Item item) { return items.FindIndex(it => it == item); }

    public bool Contains (Item item) { return items.Any(it => it == item); }
}
