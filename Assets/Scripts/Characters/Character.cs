using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Inventory")] [SerializeField] 
    private Inventory inventory;

    [Header("Character Stats")] [SerializeField]
    private CharacterStats characterStats;

    private void Awake ()
    {
        inventory = GetComponent<Inventory>();
    }

    public Inventory GetInventory (){ return inventory; }

    public CharacterStats GetStatistics() { return characterStats; }
}
