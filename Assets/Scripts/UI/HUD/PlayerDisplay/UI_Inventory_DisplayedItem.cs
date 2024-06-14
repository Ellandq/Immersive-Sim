using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_DisplayedItem : UI_Component
{
    public Action<int> onSelect;
    
    [Header("Item Info")] 
    private Item item;
    private bool isSelected;
    private int id;

    [Header("Object References")] 
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject itemCountObject;
    [SerializeField] private Text itemCount;
    [SerializeField] private GameObject isFavouriteObject;
    [SerializeField] private Outline outline; 
    
    public void SetUp(Item item, int id)
    {
        this.id = id;
        isSelected = false;
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
        else
        {
            itemCountObject.SetActive(false);
        }

        isFavouriteObject.SetActive(this.item.IsFavourite);
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
        onSelect = null;
        if (item == null) return;
        if (item.ItemData.IsStackable) item.onItemCountChange -= ChangeItemCountDisplay;
    }
    
    public void Select()
    {
        if (isSelected) return;
        isSelected = true;
        outline.enabled = true;
        onSelect.Invoke(id);
    }

    public void Deselect()
    {
        if (!isSelected) return;
        outline.enabled = false;
        isSelected = false;
    }
}
