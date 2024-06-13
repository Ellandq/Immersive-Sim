using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_DisplayedItem : UI_Component
{
    [Header("Item Info")] 
    private Item item;

    [Header("Object References")] 
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject itemCountObject;
    [SerializeField] private Text itemCount;
    [SerializeField] private GameObject isFavouriteObject;
    
    public void SetUp(Item item)
    {
        if (item == null)
        {
            Destroy(gameObject);
            return;
        }
        
        this.item = item;

        itemIcon.sprite = item.ItemData.Icon;

        if (this.item.ItemData.IsStackable)
        {
            this.item.onItemCountChange += ChangeItemCountDisplay;
            itemCountObject.SetActive(true);
            ChangeItemCountDisplay();
        }

        if (this.item.IsFavourite)
        {
            isFavouriteObject.SetActive(true);
        }
        else
        {
            isFavouriteObject.SetActive(false);
        }
    }

    private void ChangeItemCountDisplay()
    {
        if (item.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        itemCount.text = Convert.ToString(item.Count);
    }

    private void OnDisable()
    {
        if (item == null) return;
        if (item.ItemData.IsStackable) item.onItemCountChange -= ChangeItemCountDisplay;
    }

    private void OnDestroy()
    {
        if (item == null) return;
        if (item.ItemData.IsStackable) item.onItemCountChange -= ChangeItemCountDisplay;
    }
}
