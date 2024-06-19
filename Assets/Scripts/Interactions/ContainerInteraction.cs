using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerInteraction : EntityInteraction
{
    [SerializeField] private ItemHolder itemHolder;

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        if (itemHolder == null) itemHolder = GetComponentInParent<ItemHolder>();
        if (itemHolder == null) return;
        interactionType = itemHolder.containerType == ContainerType.Quiver ? InteractionType.PickUp : InteractionType.OpenContainer;
        SetTagIfNeeded("Interactable");
    }   

    public override void Interact(Player player)
    {
        if (itemHolder.containerType == ContainerType.Quiver)
        {
            player.GetInventory().AddItems(itemHolder.GetItems());
            Destroy(itemHolder.gameObject);
        }
        else
        {
            Debug.Log("Interacting with container");
        }
    }

    public override string GetInteractionInfo()
    {
        if (interactionType == InteractionType.OpenContainer)
        {
            return "Open " + itemHolder.containerType;
        }
        return $"Pick Up {itemHolder.GetName()} ({itemHolder.GetArrowCount()})";
    }
}