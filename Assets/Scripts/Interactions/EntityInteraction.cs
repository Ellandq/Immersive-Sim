using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInteraction : MonoBehaviour
{
    [Header("Interaction Settings")] 
    [SerializeField] protected InteractionType interactionType;

    public virtual void Interact(Player player)
    {
        Debug.LogError("Interaction not specified on object: " + gameObject.name);
    }

    public InteractionType GetInteractionType() { return interactionType; }

    public virtual string GetInteractionInfo()
    {
        return "Not defined";
    }
    
    protected void SetTagIfNeeded(string tag)
    {
        if (!gameObject.CompareTag(tag))
        {
            gameObject.tag = tag;
        }
    }
}

public enum InteractionType
{
    PickUp, OpenContainer,
    
    OpenDoor, EnterLocation,
    
    Talk,
    
    Light,
    
    Custom
    
}
