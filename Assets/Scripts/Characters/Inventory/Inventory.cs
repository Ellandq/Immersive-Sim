using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Inventory : MonoBehaviour
{
    [Header("Currency")]
    [SerializeField] private long gold;
    
    [Header("Inventory")]
    private Dictionary<ItemType, ItemCollection> items;
    [SerializeField] private List<ItemCollection> itemCollections;
    
    private void Awake ()
    {
        items = new Dictionary<ItemType, ItemCollection>();
        
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            items.Add(type, itemCollections[(int)type]);
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

    #region Item Manipulation

        public void AddItem (Item item)
        {
            items[item.GetItemType()].AddItem(item);
        }

        public void AddItems (List<Item> items)
        {
            foreach (var item in items) AddItem(item);
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

    #endregion

    #region Getters/Setters

        public Dictionary<ItemType, ItemCollection> GetItems() { return items; }

        public ItemCollection GetCollection(ItemType itemType) { return items[itemType]; }

    #endregion

    #region Editor
    
        private void OnValidate()
        {
            InitializeInventory();
        }

        [ContextMenu("Reset Inventory")]
        private void ResetInventory()
        {
            if (itemCollections.Count != 0) itemCollections.Clear();
            InitializeInventory();
            gold = 0;
        }
    
    #endregion
   
}
