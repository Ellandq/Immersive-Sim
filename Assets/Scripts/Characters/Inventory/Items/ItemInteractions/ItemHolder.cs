using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    private List<Item> items;
    [SerializeField] private List<ItemObject> itemObjects;
    [SerializeField] private List<int> itemCounts;

    private void Start ()
    {
        items = new List<Item>();
        
        int index = 0;
        foreach (ItemObject obj in itemObjects)
        {
            if (itemCounts.Count < index) itemCounts.Add(1);
            items.Add(new Item(obj, itemCounts[index]));
            index++;
        }
    }

    public List<Item> GetItems () { return items; }
}
