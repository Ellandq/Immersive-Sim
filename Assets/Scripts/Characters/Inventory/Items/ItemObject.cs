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
    [SerializeField] private GameObject itemPrefab;

    public GameObject Prefab {
        get { return itemPrefab; }
    }

    public Sprite Icon {
        get { return itemIcon; }
    }

    public ItemType Type {
        get { return itemType; }
    }

    public string Name {
        get { return itemName; }
    }

    public string Collection {
        get { return collection; }
    }

    public string Description {
        get { return itemDescription; }
    }

    public int GoldValue {
        get { return goldValue; }
    }

    public bool IsStackable {
        get { return isStackable; }
    }

    public override string ToString (){
        return collection + "_" + hiddenName;
    }
}
