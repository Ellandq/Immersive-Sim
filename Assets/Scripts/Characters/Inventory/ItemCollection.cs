using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemCollection 
{
    [SerializeField] private ItemType collectionType;
    [SerializeField] private List<Item> items;

    public ItemCollection (ItemType collectionType)
    {
        this.collectionType = collectionType;
        items = new List<Item>();
    }

    #region Collection Manipulation

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

            if (!item.ItemData.IsStackable || !Contains(item)) return;
            var index = GetSameItemIndex(item);
            items[index] -= item;
            if (items[index].IsEmpty()) items.RemoveAt(index);
        }

        public void SortCollection ()
        {
            foreach (var item in items)
            {
                // TODO
            }
        }

    #endregion

    #region Getters

        public List<Item> GetItems() { return items; }

        public ItemType GetCollectionType() { return collectionType; }

        public int GetSameItemIndex (Item item) { return items.FindIndex(it => it == item); }

        public bool Contains (Item item) { return items.Any(it => it == item); }

    #endregion
}
