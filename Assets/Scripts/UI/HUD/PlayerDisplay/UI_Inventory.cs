using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory : UI_Component
{
    private Inventory inventory;
    
    private List<Item> currentDisplayedItems;

    private bool isSetUp;
    private bool isSectionSetUp;

    [Header("Object References")] 
    [SerializeField] private UI_StatDisplay healthDisplay;
    [SerializeField] private List<GameObject> subsectionButtons;
    private int currentOpenSection;
    private int? currentSelectedItem;

    [Header("Item Rows")] 
    [SerializeField] private Transform itemDisplayParent;
    [SerializeField] private GameObject itemHolderPrefab;
    private List<UI_Inventory_DisplayedItem> items;

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
        items = new List<UI_Inventory_DisplayedItem>();
        SetUpInventory();
    }

    public void SetUpInventory()
    {
        subsectionButtons[currentOpenSection].gameObject.SetActive(false);
        currentOpenSection = 4;
        subsectionButtons[currentOpenSection].gameObject.SetActive(true);
        isSetUp = true;
        ClearRows();
        foreach (ItemSection section in Enum.GetValues(typeof(ItemSection))) SetUpInventoryBySection((int)section);
        isSetUp = false;
        CreateItemDisplay();
    }
    
    public void SetUpInventoryBySection(int itemSection)
    {
        if (!isSetUp)
        {
            ClearRows();
            subsectionButtons[currentOpenSection].gameObject.SetActive(false);
            currentOpenSection = itemSection;
            subsectionButtons[currentOpenSection].gameObject.SetActive(true);
        }
        isSectionSetUp = true;
        foreach (var type in ItemManager.GetItemTypes((ItemSection)itemSection)) SetUpInventoryByType((int)type);
        isSectionSetUp = false;
        if (!isSetUp) CreateItemDisplay();
    }
    
    public void SetUpInventoryByType(int itemType)
    {
        if (!isSectionSetUp && !isSetUp) ClearRows();
        currentDisplayedItems.AddRange(SortItems(inventory.GetCollection((ItemType)itemType).GetItems()));
        if (!isSectionSetUp && !isSetUp) CreateItemDisplay();
    }

    private List<Item> SortItems(List<Item> items)
    {
        // TODO Implement sorting system
        return items;
    }

    private void CreateItemDisplay()
    {
        var index = 0;
        foreach (var rowItems in currentDisplayedItems)
        {
            items.Add(Instantiate(itemHolderPrefab, itemDisplayParent.position, itemDisplayParent.rotation, itemDisplayParent)
                .GetComponent<UI_Inventory_DisplayedItem>());
            items[index].SetUp(rowItems, index);
            items[index].onSelect += SelectedItemChange;
            index++;
        }
    }

    public void ClearRows()
    {
        currentSelectedItem = null;
        currentDisplayedItems = new List<Item>();
        foreach (var row in items)
        {
            Destroy(row.gameObject);
        }
        items.Clear();
    }

    private void SelectedItemChange(int index)
    {
        if (currentSelectedItem != null)
        {
            items[(int)currentSelectedItem].Deselect();
        }
        currentSelectedItem = index;
    }

    private void HandleFavouriteSelection()
    {
        if (!enabled || currentSelectedItem == null) return;

        if (items[(int)currentSelectedItem].ChangeFavouriteStatus())
        {
            
        }
    }
}

public enum SortType
{
    
}
