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

    private bool isSetUp = false;
    private bool isSectionSetUp = false;

    [Header("Object References")] 
    [SerializeField] private UI_StatDisplay healthDisplay;

    [Header("Item Rows")] 
    [SerializeField] private Transform itemDisplayParent;
    [SerializeField] private GameObject itemRowPrefab;
    private List<UI_Inventory_ItemRow> itemRows;

    [Header("Settings")] 
    private SortType sortType;
    
    public override void EnableComponent(bool instant = true)
    {
        healthDisplay.SetToStay(true);
        base.EnableComponent(instant);
    }
    
    public override void DisableComponent(bool instant = true)
    {
        healthDisplay.SetToStay(false);
        ClearRows();
        base.DisableComponent(instant);
    }
    
    public void SetUp()
    {
        inventory = PlayerManager.GetPlayer().GetInventory();
        currentDisplayedItems = new List<Item>();
        itemRows = new List<UI_Inventory_ItemRow>();
        SetUpInventory();
        SetUpSelection();
    }

    public void SetUpInventory()
    {
        isSetUp = true;
        ClearRows();
        foreach (ItemSection section in Enum.GetValues(typeof(ItemSection))) SetUpInventoryBySection((int)section);
        isSetUp = false;
    }
    
    public void SetUpInventoryBySection(int itemSection)
    {
        if (!isSetUp) ClearRows();
        isSectionSetUp = true;
        foreach (var type in ItemManager.GetItemTypes((ItemSection)itemSection)) SetUpInventoryByType((int)type);
        isSectionSetUp = false;
    }
    
    public void SetUpInventoryByType(int itemType)
    {
        if (!isSectionSetUp && !isSetUp) ClearRows();
        currentDisplayedItems.AddRange(SortItems(inventory.GetCollection((ItemType)itemType).GetItems()));
    }

    private List<Item> SortItems(List<Item> items)
    {
        // TODO Implement sorting system
        return items;
    }

    public void SetUpSelection()
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
            itemRows.Add(Instantiate(itemRowPrefab, itemDisplayParent.position, itemDisplayParent.rotation, itemDisplayParent)
                .GetComponent<UI_Inventory_ItemRow>());
            itemRows[index].SetUp(rowItems);
            index++;
        }
    }

    public void ClearRows()
    {
        currentDisplayedItems = new List<Item>();
        foreach (var row in itemRows)
        {
            Destroy(row.gameObject);
        }
        itemRows.Clear();
    }
}

public enum SortType
{
    
}
