using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [Header("Item Information")]
    private List<Item> items;
    [SerializeField] private List<ItemData> itemDatas;
    
    [Header("Container Information")]
    [SerializeField] private ContainerType containerType;
    [SerializeField] private bool isContainer;

    private void Start()
    {
        items = new List<Item>();
        
        foreach (var obj in itemDatas)
        {
            items.Add(new Item(obj.ItemObject, obj.ItemCount));
        }
    }

    public List<Item> GetItems() { return items; }

    private void OnDestroy()
    {
        try
        {
            InputManager.GetMouseHandle().CheckForObjectRemoval(transform.GetChild(0).GetComponent<EntityInteraction>());
        }
        catch (NullReferenceException e)
        {
            Debug.LogError(e.Message);
        }
        
    }

    [ContextMenu("Update Object")]
    private void UpdateModel()
    {
        RemoveModel();

        switch (itemDatas.Count)
        {
            case (0):
                UpdateName("Item");
                return;
            
            case (1):
                Instantiate(itemDatas[0].ItemObject.Prefab, transform).AddComponent<ItemInteraction>();
                UpdateName(itemDatas[0].ItemObject.Name);
                break;
                
            default:
                Instantiate(ItemManager.GetContainer(containerType), transform).AddComponent<ContainerInteraction>();
                UpdateName(containerType.ToString());
                break;
        }
    }

    [ContextMenu("Stack Items")]
    private void StackItems()
    {
        foreach (var obj in itemDatas)
        {
            items.Add(new Item(obj.ItemObject, obj.ItemCount));
        }
    }
    
    [ContextMenu("Sort Items")]
    private void SortItems()
    {
        foreach (var obj in itemDatas)
        {
            items.Add(new Item(obj.ItemObject, obj.ItemCount));
        }
    }
    

    private void RemoveModel()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void UpdateName(string name)
    {
        gameObject.name = name;
    }
    
}
