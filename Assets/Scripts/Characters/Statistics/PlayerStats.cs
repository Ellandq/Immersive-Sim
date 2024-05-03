using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [Header("Events")] 
    // HEALTH
    private Action<float> onHealthChange;
    private Action<float> onMaxHealthChange; 
    
    // STAMINA
    private Action<float> onStaminaChange; 
    private Action<float> onMaxStaminaChange; 
    private Action<float> onStaminaUseMultiplierChange;
    
    // MANA
    private Action<float> onManaChange;
    private Action<float> onMaxManaChange; 
    // LEVEL
    private Action<float> onLevelUp;

    [Header ("Stamina Regeneration")]
    private Coroutine staminaRegenerationCoroutine;
    private Coroutine staminaRegenerationCooldownCoroutine;
    private const float staminaRegenerationCooldown = 2f;
    private float staminaRegenerationCooldownValue = 0f;
    
    public void SetUp()
    {
        // Here the player stats should be updated in accordance to the loaded data
    }

    private void Start()
    {
        PlayerManager.SubscribeToOnPlayerJump(JumpDrain);
        PlayerManager.SubscribeToOnPlayerSprint(SprintDrain);
        
        // HEALTH
        onHealthChange?.Invoke(health);
        onMaxHealthChange?.Invoke(maxHealth);
    
        // STAMINA
        onStaminaChange?.Invoke(stamina);
        onMaxStaminaChange?.Invoke(maxStamina);
        onStaminaUseMultiplierChange?.Invoke(StaminaUseMultiplier);
    
        // MANA
        onManaChange?.Invoke(mana);
        onMaxManaChange?.Invoke(maxMana);
    }

    #region Listeners Assignment

        public void AddStaminaUseMultiplierChangeListener(Action<float> listener)
        {
            onStaminaUseMultiplierChange += listener;
        }
        
        public void AddStaminaChangeListener(Action<float> listener)
        {
            onStaminaChange += listener;
        }
        
        public void AddStatChangeListener(StatType statType, Action<float> listener)
        {
            switch (statType)
            {
                case StatType.Health:
                    onHealthChange += listener;
                    break;
                case StatType.Stamina:
                    onStaminaChange += listener;
                    break;
                case StatType.Mana:
                    onManaChange += listener;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
            }
        }
        
        public void AddMaxStatChangeListener(StatType statType, Action<float> listener)
        {
            switch (statType)
            {
                case StatType.Health:
                    onMaxHealthChange += listener;
                    break;
                case StatType.Stamina:
                    onMaxStaminaChange += listener;
                    break;
                case StatType.Mana:
                    onMaxManaChange += listener;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
            }
        }

    #endregion

    #region Stamina
        
        private void SprintDrain()
        {
            stamina = Mathf.Clamp(
                stamina - SprintStaminaDrainPerSecond * StaminaUseMultiplier * Time.deltaTime, 0f, maxStamina
                );
            onStaminaChange?.Invoke(stamina);
            HandleStaminaChanges();
        }
        
        private void JumpDrain()
        {
            stamina = Mathf.Clamp(
                stamina - JumpStaminaDrain, 0f, maxStamina
            );
            onStaminaChange?.Invoke(stamina);
            HandleStaminaChanges();
        }

        private void HandleStaminaChanges()
        {
            if (staminaRegenerationCoroutine == null)
            {
                if (staminaRegenerationCooldownCoroutine == null)
                {
                    staminaRegenerationCooldownCoroutine = StartCoroutine(StaminaRegenerationCooldown());
                }
                else
                {
                    staminaRegenerationCooldownValue = staminaRegenerationCooldown;
                }
            }
            else
            {
                StopCoroutine(staminaRegenerationCoroutine);
                staminaRegenerationCoroutine = null;
                staminaRegenerationCooldownCoroutine = StartCoroutine(StaminaRegenerationCooldown());
            }
        }

    #endregion

    private IEnumerator StaminaRegeneration()
    {
        while (stamina < maxStamina)
        {
            yield return null;
            stamina = Mathf.MoveTowards(stamina, maxStamina, StaminaRegenerationPerSecond * maxStamina * Time.deltaTime);
            onStaminaChange?.Invoke(stamina);
        }

        staminaRegenerationCoroutine = null;
    }
    
    private IEnumerator StaminaRegenerationCooldown()
    {
        staminaRegenerationCooldownValue = staminaRegenerationCooldown;
        while (staminaRegenerationCooldownValue >= 0)
        {
            yield return new WaitForSeconds(1f);
            staminaRegenerationCooldownValue--;
        }

        staminaRegenerationCooldownCoroutine = null;
        staminaRegenerationCoroutine = StartCoroutine(StaminaRegeneration());
    }
}