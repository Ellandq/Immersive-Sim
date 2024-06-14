using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacter
{
    [Header ("Core Components")]
    [SerializeField] private PlayerMovement movementHandle;
    [SerializeField] private PlayerInteraction interactionHandle;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Rigidbody playerBody;

    [Header("Cameras")] 
    [SerializeField] private FirstPersonCamera firstPersonCamera;
    [SerializeField] private FirstPersonCamera thirdPersonCamera;
    

    public PlayerMovement GetMovementHandle() { return movementHandle; }
    
    public PlayerInteraction GetInteractionHandle() { return interactionHandle; }
    
    public Inventory GetInventory (){ return playerInventory; }

    public CharacterStats GetStatistics() { return playerStats; }

    public FirstPersonCamera GetFirstPersonCamera() { return firstPersonCamera; }
    
    public FirstPersonCamera GetThirdPersonCamera() { return thirdPersonCamera; }

    public Rigidbody GetPlayerBody() { return playerBody; }
}   
