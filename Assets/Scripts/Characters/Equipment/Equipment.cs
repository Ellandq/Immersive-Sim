using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [Header("Hands")] 
    // Right Hand
    [SerializeField] private HeldItem RightHand;
    [SerializeField] private Transform rightHandTransform;
    // Left Hand
    [SerializeField] private HeldItem LeftHand;
    [SerializeField] private Transform leftHandTransform;

    [Header("Equipment Details")] 
    [SerializeField] private FavoriteItems favoriteItems;
    
    public FavoriteItems GetFavoriteItemsHandle() { return favoriteItems; }

    public void Awake()
    {
        
    }

    private void TEST_EquipBow()
    {
        
    }
}
