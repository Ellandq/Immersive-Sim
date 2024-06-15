using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item Object")]
public class ItemObject : ScriptableObject
{
    [Header ("Item Basic Information")]
    [HideInInspector]
    public string ID;
    public string ItemName;
    public string HiddenName;
    public string Collection;
    
    [Header ("Item Detailed Information")]
    public ItemType ItemType;
    public string ItemDescription;
    public int GoldValue;
    public bool IsStackable;

    [Header ("Item Visuals")]
    public Sprite ItemIcon;
    
    private void OnEnable()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = System.Guid.NewGuid().ToString();
        }
    }

    public override string ToString ()
    {
        return Collection + HiddenName;
    }
}