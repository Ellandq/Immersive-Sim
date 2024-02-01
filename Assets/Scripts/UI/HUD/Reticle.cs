using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Reticle : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private List<RectTransform> reticleParts;
    [SerializeField] private TMP_Text interactionInfoDisplay;

    [Header ("Reticle movement settings")]
    [SerializeField] private float startingDistance;
    [SerializeField] private float hoverDistance;
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float allowedOffset;
    
    private bool hoveringOutwards;

    private Coroutine hoverCoroutine;
    private Coroutine positionAdjustment;
    private Coroutine interactionInfoDisplayCoroutine;

    private EntityInteraction selectedObject;

    private void Start ()
    {
        hoveringOutwards = true;
        InputManager.GetMouseHandle().AddListenerOnObjectChange(HandleObjectSelectionChange);
    }
    
    private void HandleObjectSelectionChange(EntityInteraction obj)
    {
        selectedObject = obj;
        ChangeHoveringState();
        ChangeInteractionDisplayVisibility();
    }

    #region Reticle

        private void ChangeHoveringState ()
        {
            if (ReferenceEquals(selectedObject, null))
            {
                if (hoverCoroutine != null)
                {
                    StopCoroutine(hoverCoroutine);
                    ResetPosition();
                    hoverCoroutine = null;
                }
            }
            else if (hoverCoroutine == null)
            {
               hoverCoroutine = StartCoroutine(Hover());
            }
        }

        private void ResetPosition ()
        {
            positionAdjustment =  StartCoroutine(ReturnPosition());
        }

        private void EndPositionAdjustment ()
        {
            positionAdjustment = null;
        }

        private bool MovingOutwards ()
        {
            Vector3 startPos = reticleParts[0].localPosition.normalized * (startingDistance + allowedOffset);
            Vector3 endPos = reticleParts[0].localPosition.normalized * (startingDistance + hoverDistance - allowedOffset);
            return hoveringOutwards ? reticleParts[0].localPosition.x <= endPos.x : reticleParts[0].localPosition.x <= startPos.x;
        }

        private IEnumerator Hover()
        {
            float t = 0f;    

            while (true)
            {
                hoveringOutwards = MovingOutwards();

                if (hoveringOutwards)
                {
                    t = Mathf.Lerp(t, hoverDistance, hoverSpeed * Time.deltaTime);
                }
                else 
                {
                    t = Mathf.Lerp(t, 0f, hoverSpeed * Time.deltaTime);
                }

                foreach (RectTransform rectT in reticleParts)
                {
                    var localPosition = rectT.localPosition;
                    localPosition = localPosition.normalized * startingDistance + t * localPosition.normalized;
                    rectT.localPosition = localPosition;
                }

                yield return null;
            }
        }

        private IEnumerator ReturnPosition()
        {
            var targetPos = reticleParts[0].localPosition.normalized * startingDistance;
            while ((reticleParts[0].localPosition.magnitude  - targetPos.magnitude) > 1f)
            {
                foreach (RectTransform rectT in reticleParts)
                {
                    Vector3 localPosition = rectT.localPosition;
                    Vector3 startingPos = localPosition.normalized * 50f;

                    float t = Mathf.SmoothStep(0f, 1f, Time.time * hoverSpeed);

                    localPosition = Vector3.Lerp(localPosition, startingPos, t * Time.deltaTime * 5f);
                    rectT.localPosition = localPosition;
                }

                yield return null;
            }
            
            foreach (RectTransform rectT in reticleParts)
            {
                rectT.localPosition = rectT.localPosition.normalized * 50f;
            }

            EndPositionAdjustment();
        }
    
    #endregion


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
