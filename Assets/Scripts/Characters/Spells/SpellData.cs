using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "ScriptableObjects/SpellData")]
public class SpellData : ScriptableObject
{
    [Header("Basic Info")] 
    public string spellName;
    public SpellType spellType;
    public List<ElementalType> elementalTypes;

    [Header("Spell ")] 
    public float intensity;
    public float magnitude;
    
}
