using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Inventory : MonoBehaviour
{
    [SerializeField] private long gold;
    
    [Header("Inventory")]
    private Dictionary<ItemType, ItemCollection> items;
    [SerializeField] private List<ItemCollection> itemCollections;
    
    private void Awake ()
    {
        items = new Dictionary<ItemType, ItemCollection>();

        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            items[type] = itemCollections[(int)type];
        }
    }
    
    private void InitializeInventory ()
    {
        if (itemCollections.Count != 0) return; 
        
        itemCollections = new List<ItemCollection>();

        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            itemCollections.Add(new ItemCollection(type));
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

    public Dictionary<ItemType, ItemCollection> GetItems() { return items; }

    private void OnValidate()
    {
        InitializeInventory();
    }
}