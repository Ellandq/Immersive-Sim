using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    [SerializeField] private ItemObject itemObject;
    [SerializeField] private int itemCount;

    public ItemObject ItemObject { get { return itemObject; } }
    public int ItemCount { get { return itemCount; } }

    public ItemData(ItemObject itemObject, int itemCount)
    {
        this.itemObject = itemObject;
        this.itemCount = itemCount;
    }
}
