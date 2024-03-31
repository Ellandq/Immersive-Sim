using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, ICharacter
{
    [Header("Inventory")]
    private Inventory inventory;

    [Header("Character Stats")]
    private CharacterStats characterStats;

    private void Awake ()
    {
        inventory = GetComponent<Inventory>();
        characterStats = GetComponent<CharacterStats>();
    }

    public Inventory GetInventory (){ return inventory; }

    public CharacterStats GetStatistics() { return characterStats; }
}
