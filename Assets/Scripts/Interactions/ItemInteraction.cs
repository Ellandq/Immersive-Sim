using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : EntityInteraction
{

    [SerializeField] private ItemHolder itemHolder;
    
    private void OnValidate()
    {
        if (itemHolder == null) itemHolder = GetComponentInParent<ItemHolder>();

        interactionType = InteractionType.PickUp;
        gameObject.tag = "Interactable";
    }
    
    public override void Interact(Player player)
    {
        player.GetInventory().AddItems(itemHolder.GetItems());
        
        Destroy(itemHolder.gameObject);
    }
    
    public override string GetInteractionInfo()
    {
        return $"Pick Up {itemHolder.GetName()}";
    }
}
