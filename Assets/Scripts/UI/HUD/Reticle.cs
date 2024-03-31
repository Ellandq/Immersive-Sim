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
                if (hoverCoroutine == null) return;
                StopCoroutine(hoverCoroutine);
                ResetPosition();
                hoverCoroutine = null;
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
            hoveringOutwards = true;
        }

        private void MovingOutwards ()
        {
            var currentPos = Mathf.Abs(reticleParts[0].localPosition.x);
            hoveringOutwards = hoveringOutwards ? currentPos < startingDistance + hoverDistance : startingDistance >= currentPos;
        }

        private IEnumerator Hover()
        {
            var moveValue = 0f;    

            while (true)
            {
                MovingOutwards();
                var hoverPos = hoveringOutwards ? hoverDistance : 0f;   
                
                moveValue = Mathf.MoveTowards(
                    Mathf.SmoothStep(moveValue, hoverPos, hoverSpeed * Time.deltaTime), 
                    hoverPos, hoverSpeed * Time.deltaTime * 5f
                    );

                foreach (var rectT in reticleParts)
                {
                    var localPosition = rectT.localPosition;
                    localPosition = localPosition.normalized * startingDistance + moveValue * localPosition.normalized;
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
                foreach (var rectT in reticleParts)
                {
                    var localPosition = rectT.localPosition;
                    var startingPos = localPosition.normalized * 50f;

                    var t = Mathf.SmoothStep(0f, 1f, Time.time * hoverSpeed);
                    
                    localPosition = Vector3.Lerp(localPosition, startingPos, t * Time.deltaTime * 5f);
                    
                    // var moveValue = Mathf.Lerp(localPosition, )
                    rectT.localPosition = localPosition;
                }

                yield return null;
            }
            
            foreach (var rectT in reticleParts)
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
