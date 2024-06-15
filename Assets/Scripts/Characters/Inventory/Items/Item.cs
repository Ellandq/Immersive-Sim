using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public Action onItemCountChange;
    
    [Header ("Item Information")]
    [SerializeField] private ItemObject itemData;
    [SerializeField] private int count;
    [SerializeField] private bool isFavourite;

    public Item(ItemObject itemData, int count = 1)
    {
        this.count = count;
        this.itemData = itemData;
    }

    #region Operators

        public static Item operator +(Item a, Item b)
        {
            if (a != b) throw new ItemTypeException("Cannot convert from: " + a.GetId() + ", to: " + b.GetId());
            a.count += b.count;
            a.onItemCountChange?.Invoke();
            return a;
        }

        public static Item operator -(Item a, Item b)
        {
            if (a != b) throw new ItemTypeException("Cannot convert from: " + a.GetId() + ", to: " + b.GetId());
            if (a.count < b.count) throw new ItemNegativeCountException();
            a.count -= b.count;
            a.onItemCountChange?.Invoke();
            return a;
        }

        public static bool operator ==(Item a, Item b)
        {
            // If both are null, or both are the same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (a is null || b is null)
            {
                return false;
            }

            // Otherwise, compare their IDs.
            return a.GetId() == b.GetId();
        }

        public static bool operator !=(Item a, Item b)
        {
            return !(a == b);
        }

        public static bool operator ==(Item a, ItemType b)
        {
            if (a is null)
            {
                return false;
            }

            return a.GetItemType() == b;
        }

        public static bool operator !=(Item a, ItemType b)
        {
            return !(a == b);
        }

        // Override Equals and GetHashCode when overloading == and !=
        public override bool Equals(object obj)
        {
            if (obj is Item item)
            {
                return this == item;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetId().GetHashCode();
        }

    #endregion

    #region Getters

        public ItemObject ItemData { get { return itemData; } }
        
        public ItemType GetItemType () { return itemData.ItemType; }
        
        public string GetId () { return itemData.ToString(); }

        public int Count { get { return count; } }

        public bool IsFavourite { get { return isFavourite; } }

        public bool IsEmpty () { return count == 0; }

    #endregion
}
