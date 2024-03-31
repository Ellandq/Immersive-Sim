using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Basic info")] 
    // HEALTH
    [SerializeField] private float maxHealth = 100f;
    private float Health { get; set; }
    // STAMINA
    [SerializeField] private float maxStamina = 100f;
    private float Stamina{ get; set; }

    [Header("Damage Multipliers")] 
    private Dictionary<ElementalType, float> elementalDamageMultipliers;
    private Dictionary<DamageType, float> damageTypeMultipler;
    

    [Header("Defense Multipliers")]
    private Dictionary<ElementalType, float> elementalDefenseMultipliers;
    private Dictionary<DamageType, float> defenseTypeMultipler;
    
    private void Awake()
    {
        elementalDamageMultipliers = new Dictionary<ElementalType, float>();
        elementalDefenseMultipliers = new Dictionary<ElementalType, float>();
        
        foreach (ElementalType value in Enum.GetValues(typeof(ElementalType)))
        {
            elementalDamageMultipliers.Add(value, 1f);
            elementalDefenseMultipliers.Add(value, 1f);
        }
    }

    public float GetElementalDamageMultiplier(ElementalType type) { return elementalDamageMultipliers[type]; }
    
    public float GetElementalDefenseMultiplier(ElementalType type) { return elementalDamageMultipliers[type]; }
    
    public float GetDamageTypeMultiplier(DamageType type) { return damageTypeMultipler[type]; }
    
    public float GetDefenseTypeMultiplier(DamageType type) { return defenseTypeMultipler[type]; }
    
    
}
