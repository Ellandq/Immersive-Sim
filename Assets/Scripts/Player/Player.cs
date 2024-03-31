using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacter
{
    [SerializeField] private PlayerMovement movementHandle;
    [SerializeField] private PlayerInteraction interactionHandle;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Inventory playerInventory;
    
    public Inventory GetInventory (){ return playerInventory; }

    public CharacterStats GetStatistics() { return playerStats; }





}   
