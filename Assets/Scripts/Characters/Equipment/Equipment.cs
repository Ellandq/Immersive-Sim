using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [Header("Hands")] 
    [SerializeField] private HeldItem RightHand;
    [SerializeField] private HeldItem LeftHand;

    [Header("Equipment Details")] 
    [SerializeField] private FavoriteItems favoriteItems;



    public FavoriteItems GetFavoriteItemsHandle() { return favoriteItems; }
}
