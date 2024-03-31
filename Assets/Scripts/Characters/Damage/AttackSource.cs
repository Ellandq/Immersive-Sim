using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSource : MonoBehaviour
{
    [Header("Attack Source Information")]
    [SerializeField] protected ElementalType elementalType;
    [SerializeField] protected DamageType damageType;
    [SerializeField] private float baseDamageValue;

    public ElementalType GetElementalType() { return elementalType; }
    
    public DamageType GetDamageType() { return damageType; }

    public float GetBaseDamageValue() { return baseDamageValue; }
}


