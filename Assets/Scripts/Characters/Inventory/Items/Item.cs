using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int Count{ get; }
    public ItemObject ItemData { get;}
    public bool IsFavourite { get; }

    public Item(ItemObject itemData, int count = 1)
    {
        Count = count;
        ItemData = itemData;
    }

    public static Item operator +(Item a, Item b)
    {
        if (a != b) throw new ItemTypeException("Cannot convert from: " + a.GetId() + ", to: " + b.GetId());
        return new Item(a.ItemData, a.Count + b.Count);
    }

    public static Item operator -(Item a, Item b)
    {
        if (a != b) throw new ItemTypeException("Cannot convert from: " + a.GetId() + ", to: " + b.GetId());
        if (a.Count < b.Count) throw new ItemNegativeCountException();
        return new Item(a.ItemData, a.Count - b.Count);
    }

    public static bool operator ==(Item a, Item b){
        return a.GetId() == b.GetId();
    }

    public static bool operator !=(Item a, Item b){
        return a.GetId() != b.GetId();
    }

    public static bool operator ==(Item a, ItemType b){
        return a.GetItemType() == b;
    }

    public static bool operator !=(Item a, ItemType b){
        return a.GetItemType() != b;
    }

    public string GetId () { return ItemData.ToString(); }

    public ItemType GetItemType () { return ItemData.Type; }

    public bool IsEmpty () { return Count == 0; }
}
