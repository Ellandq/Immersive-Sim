using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [SerializeField] private List<RectTransform> reticleParts;

    [SerializeField] private float startingDistance;
    [SerializeField] private float hoverDistance;
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float allowedOffset;
    
    private bool hoveringOutwards;

    private Coroutine hoverCoroutine;
    private Coroutine positionAdjustment;

    private void Start ()
    {
        hoveringOutwards = true;
        InputManager.GetMouseHandle().onSelectedObjectChange += ChangeHoveringState;
    }

    public void ChangeHoveringState (GameObject obj)
    {
        if (obj == null)
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

    private bool IsMovingOutwards ()
    {
        Vector3 startPos = reticleParts[0].localPosition.normalized * (startingDistance + allowedOffset);
        Vector3 endPos = reticleParts[0].localPosition.normalized * (startingDistance + hoverDistance - allowedOffset);
        return hoveringOutwards ? reticleParts[0].localPosition.x <= endPos.x : reticleParts[0].localPosition.x <= startPos.x;
    }

    public IEnumerator Hover()
    {
        float t = 0f;    

        while (true)
        {
            hoveringOutwards = IsMovingOutwards();

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
                rectT.localPosition = rectT.localPosition.normalized * startingDistance + t * rectT.localPosition.normalized;
            }

            yield return null;
        }
    }

    public IEnumerator ReturnPosition()
    {
        while (reticleParts[0].localPosition != reticleParts[0].localPosition.normalized * 50f)
        {
            foreach (RectTransform rectT in reticleParts)
            {
                Vector3 startingPos = rectT.localPosition.normalized * 50f;

                float t = Mathf.SmoothStep(0f, 1f, Time.time * hoverSpeed);

                rectT.localPosition = Vector3.Lerp(rectT.localPosition, startingPos, t * Time.deltaTime);
            }

            yield return null;
        }

        EndPositionAdjustment();
    }





}
