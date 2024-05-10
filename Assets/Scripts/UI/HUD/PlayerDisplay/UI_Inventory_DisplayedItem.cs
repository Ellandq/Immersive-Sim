using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_DisplayedItem : UI_Component
{
    private Item item;

    [Header("Object References")] 
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    
    public void SetUp(Item item)
    {
        this.item = item;

        text.text = item.ItemData.Name;

        image.sprite = item.ItemData.Icon;
    }
}
