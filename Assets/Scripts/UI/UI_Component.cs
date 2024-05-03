using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Component : MonoBehaviour
{
    public bool IsEnabled { get; private set; }
    [SerializeField] private CanvasGroup canvasGroup;

    [Header ("Fade Settings")]
    [SerializeField] private float fadeInSpeed = 10f;
    [SerializeField] private float fadeOutSpeed = 10f;
    [SerializeField] private float fadeOutCooldown = 6f;
    [SerializeField] protected bool useCooldown = false;
    private int fadeOutCooldownTimer = 0;
    
    [Header("Coroutines")]
    private Coroutine fadeCoroutine;
    private Coroutine fadeCooldownCoroutine;
    
    public void EnableComponent(bool instant = true)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (instant)
        {
            canvasGroup.alpha = 1f;
            IsEnabled = true;
            gameObject.SetActive(true);
            return;
        }

        fadeCoroutine =  StartCoroutine(FadeIn());
    }
    
    public void DisableComponent(bool instant = true)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (instant)
        {
            canvasGroup.alpha = 0f;
            IsEnabled = false;
            gameObject.SetActive(false);
            return;
        }

        fadeCoroutine =  StartCoroutine(FadeOut());
    }

    protected void StartFadeOutCooldown()
    {
        if (fadeCooldownCoroutine != null)
        {
            ResetFadeOutCooldown();
            return;
        }

        fadeCooldownCoroutine = StartCoroutine(FadeOutCooldown());
    }

    private void ResetFadeOutCooldown()
    {
        fadeOutCooldownTimer = 0;
    }

    private IEnumerator FadeIn()
    {
        while (canvasGroup.alpha != 1f)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.deltaTime * fadeInSpeed);
            yield return null;
        }

        IsEnabled = true;
        fadeCoroutine = null;
        if (useCooldown) StartFadeOutCooldown();
    }

    private IEnumerator FadeOut()
    {
        while (canvasGroup.alpha != 0f)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.deltaTime * fadeOutSpeed);
            yield return null;
        }

        IsEnabled = false;
        fadeCoroutine = null;
    }

    private IEnumerator FadeOutCooldown()
    {
        fadeOutCooldownTimer = 0;
        while (fadeOutCooldownTimer <= fadeOutCooldown)
        {
            yield return new WaitForSeconds(1f);
            fadeOutCooldownTimer++;
        }
        fadeCooldownCoroutine = null;
        DisableComponent(false);
    }
}
