using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory_ItemRow : UI_Component
{
    [Header("Prefab")]
    [SerializeField] private GameObject itemHolderPrefab;

    private List<UI_Inventory_DisplayedItem> itemRow;
    
    public void SetUp(List<Item> items)
    {
        var index = 0;
        ClearRow();
        itemRow = new List<UI_Inventory_DisplayedItem>();
        foreach (var itemHolder in items.Select(item =>
                     Instantiate(itemHolderPrefab, transform.position, Quaternion.identity, transform)))
        {
            itemRow.Add(itemHolder.GetComponent<UI_Inventory_DisplayedItem>());
            itemRow[index].SetUp(items[index]);
            index++;
        }
    }

    private void ClearRow()
    {
        if (itemRow == null) return;
        foreach (var itemHolder in itemRow)
        {
            Destroy(itemHolder.gameObject);
        }
    }
}
