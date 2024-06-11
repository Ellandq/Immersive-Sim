using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ConsumableItem
{
    public Item item;
    
    public ConsumableItem(Item item)
    {
        if (item.GetItemType() != ItemType.Potions
            && item.GetItemType() != ItemType.Scrolls
            && item.GetItemType() != ItemType.Runes)
        {
            throw new DataException("Invalid item type for type 'Consumable'.");
        }

        this.item = item;
    }
}
