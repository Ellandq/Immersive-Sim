using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    private void Awake ()
    {
        inventory = GetComponent<Inventory>();
    }

    public Inventory GetInventory (){ return inventory; }
}
