using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class EquippableItem
{
    public Item item;
    
    public EquippableItem(Item item)
    {
        if (item.GetItemType() != ItemType.MeleeWeapon
            && item.GetItemType() != ItemType.RangedWeapon
            && item.GetItemType() != ItemType.Staff)
        {
            throw new DataException("Invalid item type for type 'Equippable'.");
        }

        this.item = item;
    }
}
