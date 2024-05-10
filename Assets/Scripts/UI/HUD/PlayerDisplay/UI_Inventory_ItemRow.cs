using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory_ItemRow : UI_Component
{
    [SerializeField] private List<UI_Inventory_DisplayedItem> displayedItems;
    
    public void SetUp(List<Item> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            displayedItems[i].SetUp(items[i]);
        }
    }
}
