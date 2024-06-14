using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item Object")]
public class ItemObject : ScriptableObject
{
    [Header ("Item Basic Information")]
    [SerializeField] private string itemName;
    [SerializeField] private string hiddenName;
    [SerializeField] private string collection;
    
    [Header ("Item Detailed Information")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private string itemDescription;
    [SerializeField] private int goldValue;
    [SerializeField] private bool isStackable;

    [Header ("Item Visuals")]
    [SerializeField] private Sprite itemIcon;

    public Sprite Icon => itemIcon;

    public ItemType Type => itemType;

    public string Name => itemName;

    public string HiddenName => hiddenName;

    public string Collection => collection;

    public string Description => itemDescription;

    public int GoldValue => goldValue;

    public bool IsStackable => isStackable;

    public override string ToString (){ return collection + "/" + hiddenName; }
}
