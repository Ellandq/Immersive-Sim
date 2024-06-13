using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class UI_Reticle : UI_Component
{
    [Header("Object References")]
    [SerializeField] private TMP_Text interactionInfoDisplay;
    
    private Coroutine interactionInfoDisplayCoroutine;

    private EntityInteraction selectedObject;

    private void Start ()
    {
        InputManager.GetMouseHandle().AddListenerOnObjectChange(HandleObjectSelectionChange);
    }
    
    private void HandleObjectSelectionChange(EntityInteraction obj)
    {
        selectedObject = obj;
        ChangeInteractionDisplayVisibility();
    }

    #region InteractionInfo
    
        private void ChangeInteractionDisplayVisibility()
        {
            
            if (interactionInfoDisplayCoroutine != null) StopCoroutine(interactionInfoDisplayCoroutine);
            if (ReferenceEquals(selectedObject, null))
            {
                interactionInfoDisplayCoroutine = StartCoroutine(HideInteractionInfo());
                return;
            }

            interactionInfoDisplay.text =
                selectedObject.GetInteractionName() + selectedObject.GetInteractionType().ToString();
            interactionInfoDisplayCoroutine = StartCoroutine(DisplayInteractionInfo());
        }

        private IEnumerator DisplayInteractionInfo()
        {
            while (interactionInfoDisplay.alpha != 1f)
            {
                interactionInfoDisplay.alpha = Mathf.MoveTowards(interactionInfoDisplay.alpha, 1f, Time.deltaTime * 10f);
                yield return null;
            }

            interactionInfoDisplayCoroutine = null;
        }
        
        private IEnumerator HideInteractionInfo()
        {
            while (interactionInfoDisplay.alpha != 0f)
            {
                interactionInfoDisplay.alpha = Mathf.MoveTowards(interactionInfoDisplay.alpha, 0f, Time.deltaTime * 10f);
                yield return null;
            }

            interactionInfoDisplayCoroutine = null;
        }
    
    #endregion

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
