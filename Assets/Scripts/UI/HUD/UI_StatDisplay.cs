using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class UI_StatDisplay : UI_Component
{
    [Header("Object References")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text numericDisplay;
    private float value;
    private const float ValueChangeSpeed = 50f;

    private Coroutine changeValueCoroutine;

    public void SetUpDisplay(StatType statType)
    { 
        var playerStats = 
        (
            PlayerManager
            .GetPlayer()
            .GetStatistics()
            as PlayerStats
        );
        playerStats.AddStatChangeListener(statType, ChangeDisplayedValue);
        slider.maxValue = playerStats.GetMaxStatValue(statType);
        slider.value = playerStats.GetStatValue(statType);
        ChangeDisplayedValue(playerStats.GetStatValue(statType));
    }

    public void SetToStay(bool state)
    {
        numericDisplay.gameObject.SetActive(state);
        if (state)
        {
            EnableComponent(true);
            useCooldown = false;
            ResetFadeStatus();
        }
        else
        {
            DisableComponent(false);
            useCooldown = true;
        }
    }

    private void ChangeDisplayedValue(float value)
    {
        EnableComponent(false);
        this.value = value;

        numericDisplay.text = Convert.ToString(value) + " / " + Convert.ToString(slider.maxValue);

        changeValueCoroutine ??= StartCoroutine(ChangeValue());
    }

    private IEnumerator ChangeValue()
    {
        while (Math.Abs(slider.value - value) > 0.1f)
        {
            slider.value = Mathf.MoveTowards(slider.value, value, ValueChangeSpeed * Time.deltaTime);
            yield return null;
        }

        changeValueCoroutine = null;
    }
}