using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    [Header ("Item Information")]
    [SerializeField] private ItemObject itemData;
    [SerializeField] private int count;
    [SerializeField] private bool isFavourite;

    public Item(ItemObject itemData, int count = 1)
    {
        this.count = count;
        this.itemData = itemData;
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

    public static bool operator ==(Item a, Item b) { return a.GetId() == b.GetId(); }

    public static bool operator !=(Item a, Item b) { return a.GetId() != b.GetId(); }

    public static bool operator ==(Item a, ItemType b) { return a.GetItemType() == b; }

    public static bool operator !=(Item a, ItemType b) { return a.GetItemType() != b; }

    public ItemObject ItemData { get { return itemData; } }

    public int Count { get { return count; } }

    public bool IsFavourite { get { return isFavourite; } }

    public ItemType GetItemType () { return itemData.Type; }
    
    public string GetId () { return itemData.ToString(); }

    public bool IsEmpty () { return count == 0; }

    public override bool Equals(object obj) { return base.Equals (obj); }
    
    public override int GetHashCode() { return base.GetHashCode(); }
}
