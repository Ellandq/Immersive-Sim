using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("Item Prefabs")] 
    [SerializeField] private List<GameObject> containers;
    
    [ContextMenu("Set Instance")]
    private void SetInstance()
    {
        Instance = this;
    }
    
    public static GameObject GetContainer(ContainerType containerType)
    {
        if (Instance == null)
        {
            FindObjectOfType<ItemManager>().SetInstance();
        }
        return Instance.containers[(int)containerType];
    }

    public static void SortItems(List<ItemData> itemDatas)
    {
        
    }
}

public enum ItemType 
{
    // Weapons
    MeleeWeapon, RangedWeapon, Staff,
    // Ammunition
    Ammunition,
    // Consumables
    Potions, Scrolls, Runes,
    // Misc
    Plant, Ingredient, Book
}

public enum ContainerType
{
    Sack, 
    Chest
}
