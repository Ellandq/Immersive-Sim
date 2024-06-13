using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory : UI_Component
{
    private Inventory inventory;
    private List<Item> currentDisplayedItems;
    private const int MaxItemsInRow = 6;

    [Header("Item Rows")] 
    [SerializeField] private Transform itemDisplayParent;
    [SerializeField] private GameObject itemRowPrefab;
    private List<UI_Inventory_ItemRow> itemRows;

    [Header("Settings")] 
    private SortType sortType;
    
    private void Start()
    {
        inventory = PlayerManager.GetPlayer().GetInventory();
    }
    
    public void SetUp()
    {
        SetUpInventory();
    }

    public void SetUpInventory()
    {
        foreach (ItemSection section in Enum.GetValues(typeof(ItemSection))) SetUpInventory(section);
    }
    
    public void SetUpInventory(ItemSection itemSection)
    {
        foreach (var type in ItemManager.GetItemTypes(itemSection)) SetUpInventory(type);
    }
    
    public void SetUpInventory(ItemType itemType)
    {
        currentDisplayedItems.AddRange(SortItems(inventory.GetCollection(itemType).GetItems()));
        SetUpSelection();
    }

    private List<Item> SortItems(List<Item> items)
    {
        // TODO Implement sorting system
        return items;
    }

    private void SetUpSelection()
    {
        if (currentDisplayedItems.Count == 0) return;
        
        var rowItems = new List<List<Item>>();
        var tempRow = new List<Item>();

        foreach (var item in currentDisplayedItems)
        {
            tempRow.Add(item);
            if (tempRow.Count != MaxItemsInRow) continue;
            rowItems.Add(tempRow);
            tempRow = new List<Item>();
        }
        
        if (tempRow.Count > 0)
        {
            rowItems.Add(tempRow);
        }
        
        CreateItemRows(rowItems);
    }

    private void CreateItemRows(List<List<Item>> itemLists)
    {
        var index = 0;
        foreach (var rowItems in itemLists)
        {
            itemRows.Add(Instantiate(itemRowPrefab, Vector3.zero, Quaternion.identity, itemDisplayParent)
                .GetComponent<UI_Inventory_ItemRow>());
            itemRows[index].SetUp(rowItems);
            index++;
        }
    }

    private void ClearRows()
    {
        foreach (var row in itemRows)
        {
            Destroy(row.gameObject);
        }
    }
}

public enum SortType
{
    
}
