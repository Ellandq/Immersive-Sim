using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Basic info")] 
    // HEALTH
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float health;
    // STAMINA
    [SerializeField] protected float maxStamina = 100f;
    [SerializeField] protected float stamina;
    // MANA
    [SerializeField] protected float maxMana = 100f;
    [SerializeField] protected float mana;
    
    [Header("Stamina Options")] 
    protected const float JumpStaminaDrain = 10f;
    protected const float SprintStaminaDrainPerSecond = 5f;
    protected const float StaminaUseMultiplier = 1f;
    protected const float StaminaRegenerationPerSecond = 0.2f;

    [Header("Damage Multipliers")] 
    private Dictionary<ElementalType, float> elementalDamageMultipliers;
    // private Dictionary<DamageType, float> damageTypeMultipler;
    

    [Header("Defense Multipliers")]
    private Dictionary<ElementalType, float> elementalDefenseMultipliers;
    // private Dictionary<DamageType, float> defenseTypeMultipler;
    
    private void Awake()
    {
        elementalDamageMultipliers = new Dictionary<ElementalType, float>();
        elementalDefenseMultipliers = new Dictionary<ElementalType, float>();
        
        foreach (ElementalType value in Enum.GetValues(typeof(ElementalType)))
        {
            elementalDamageMultipliers.Add(value, 1f);
            elementalDefenseMultipliers.Add(value, 1f);
        }

        health = maxHealth;
        stamina = maxStamina;
        mana = maxMana;
    }

    public float GetElementalDamageMultiplier(ElementalType type) { return elementalDamageMultipliers[type]; }
    
    public float GetElementalDefenseMultiplier(ElementalType type) { return elementalDamageMultipliers[type]; }
    
    // public float GetDamageTypeMultiplier(DamageType type) { return damageTypeMultipler[type]; }
    //
    // public float GetDefenseTypeMultiplier(DamageType type) { return defenseTypeMultipler[type]; }

    public float GetMaxStatValue(StatType statType)
    {
        return statType switch
        {
            StatType.Health => maxHealth,
            StatType.Stamina => maxStamina,
            StatType.Mana => maxMana,
            _ => throw new ArgumentOutOfRangeException(nameof(statType), statType, null)
        };
    }
    
    public float GetStatValue(StatType statType)
    {
        return statType switch
        {
            StatType.Health => health,
            StatType.Stamina => stamina,
            StatType.Mana => mana,
            _ => throw new ArgumentOutOfRangeException(nameof(statType), statType, null)
        };
    }
}

public enum StatType
{
    Health, Stamina, Mana
}
