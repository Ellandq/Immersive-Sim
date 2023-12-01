using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    private List<Item> items;
    [SerializeField] private List<ItemObject> itemObjects;
    [SerializeField] private List<int> itemCounts;

    [SerializeField] private int previousItemObjectsCount;

    private void Start()
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

    public List<Item> GetItems() { return items; }

    private void OnDestroy() { InputManager.GetMouseHandle().CheckForObjectRemoval(gameObject); }

    #if UNITY_EDITOR
        private void OnValidate()
        {
            try {
                if (previousItemObjectsCount != itemObjects.Count)
                {
                    UpdateModel();

                    previousItemObjectsCount = itemObjects.Count;
                }
            }catch (Exception e){}
        }

        private void UpdateModel()
        {
            if (itemObjects.Count == 1)
            {
                GameObject obj = Instantiate(itemObjects[0].Prefab, transform);

                foreach (MeshCollider col in obj.GetComponentsInChildren<MeshCollider>())
                {
                    col.convex = true;
                    col.gameObject.layer = LayerMask.NameToLayer("Prop");
                }
            }
        }
    #endif
}
