using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class StatDisplay : UI_Component
{
    [Header("Object References")]
    [SerializeField] private Slider slider;
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

    private void ChangeDisplayedValue(float value)
    {
        EnableComponent(false);
        this.value = value;

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