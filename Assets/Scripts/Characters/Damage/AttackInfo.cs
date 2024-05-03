using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInfo
{
    [Header("Attack Information")] 
    private AttackType attackType;
    private DamageType damageType;
    private ElementalType elementalType;
    private float attackValue;

    // Case for character attacking using weapon or a spell
    public AttackInfo(CharacterStats characterStats, AttackSource attackSource, ElementalType elementalType = ElementalType.None)
    {
        this.elementalType = elementalType == ElementalType.None ? elementalType : attackSource.GetElementalType();
        damageType = attackSource.GetDamageType();
        attackType = AttackType.Immediate; // TODO
        
        // Get damage multipliers
        var damageMultiplier =  1f + characterStats.GetDamageTypeMultiplier(damageType) +
                                characterStats.GetElementalDamageMultiplier(this.elementalType);

        // Calculate attack value
        attackValue = damageMultiplier * attackSource.GetBaseDamageValue();
    }
}

public enum AttackType
{
    Immediate, OverTime, Delayed
}

public enum DamageType
{
    Piercing, Blunt, Elemental, Poison
}

public enum ElementalType
{
    None, Native, Fire, Water, Earth, Air,
}
