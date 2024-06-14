using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FavoriteItems : MonoBehaviour
{
    [SerializeField] private List<EquippableItem> favoriteEquippable;
    [SerializeField] private List<ConsumableItem> favoriteConsumable;
    [SerializeField] private List<Item> favouriteAmmunition;

    private Inventory inventory;

    private void Start()
    {
        inventory = PlayerManager
            .GetPlayer()
            .GetInventory();

        favoriteEquippable = new List<EquippableItem>();
        favouriteAmmunition = new List<Item>();
        favoriteConsumable = new List<ConsumableItem>();
    }

    public void Initialize(List<Item> favoriteItems)
    {
        foreach (var item in favoriteItems) AddItemToFavorites(item);
    }

    public void AddItemToFavorites(Item item)
    {
        switch (item.GetItemType())
        {
            // WEAPONS
            case ItemType.MeleeWeapon: 
            case ItemType.RangedWeapon:
            case ItemType.Staff:
                    favoriteEquippable.Add(new EquippableItem(item));
                break;
            
            // AMMUNITION
            case ItemType.Ammunition:
                    favouriteAmmunition.Add(item);
                break;
            
            // CONSUMABLES
            case ItemType.Potions:
            case ItemType.Scrolls:
            case ItemType.Runes:
                favoriteConsumable.Add(new ConsumableItem(item));
                break;
                
            // Other cases are dismissed
            case ItemType.LightArmor:
            case ItemType.MediumArmor:
            case ItemType.HeavyArmor:
            case ItemType.Robe:
            case ItemType.Key:
            case ItemType.Plant:
            case ItemType.Ingredient:
            case ItemType.Book:
            default:
                return;
        }
    }

    public void RemoveItemFromFavorites(Item item)
    {
        switch (item.GetItemType())
        {
            // WEAPONS
            case ItemType.MeleeWeapon: 
            case ItemType.RangedWeapon:
            case ItemType.Staff:
                favoriteEquippable.RemoveAll(equippableItem => item.ItemData.ToString() == equippableItem.item.ItemData.ToString());
                return;
            
            // AMMUNITION
            case ItemType.Ammunition:
                favouriteAmmunition.Add(item);
                break;
            
            // CONSUMABLES
            case ItemType.Potions:
            case ItemType.Scrolls:
            case ItemType.Runes:
                favoriteConsumable.Add(new ConsumableItem(item));
                break;
                
            // Other cases are dismissed
            case ItemType.LightArmor:
            case ItemType.MediumArmor:
            case ItemType.HeavyArmor:
            case ItemType.Robe:
            case ItemType.Key:
            case ItemType.Plant:
            case ItemType.Ingredient:
            case ItemType.Book:
            default:
                return;
        }
    }

    public void AddSpellToFavorites()
    {
        
    }
    
    public void RemoveSpellFromFavorites()
    {
        
    }
}
