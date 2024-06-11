using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FavoriteItems : MonoBehaviour
{
    [SerializeField] private List<EquippableItem> favoriteEquippable;
    [SerializeField] private List<ConsumableItem> favoriteConsumable;

    private Inventory inventory;

    private void Awake()
    {
        inventory = PlayerManager
            .GetPlayer()
            .GetInventory();
    }

    public void Initialize(List<Item> favoriteItems)
    {
        foreach (var item in favoriteItems)
        {
            
        }
    }

    public void AddItemToFavorites(Item item)
    {
        
    }

    public void RemoveItemFromFavorites()
    {
        
    }

    public void AddSpellToFavorites()
    {
        
    }
    
    public void RemoveSpellFromFavorites()
    {
        
    }

    private bool IsEquippable(ItemType itemType)
    {
        return (itemType == ItemType.MeleeWeapon
                || itemType == ItemType.RangedWeapon
                || itemType == ItemType.Staff);
    }
}
