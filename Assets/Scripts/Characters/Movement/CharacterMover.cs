using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    private CharacterController controller;
    private Transform groundCheck;
    private LayerMask groundMask;
    private MovementType movementType;

    private Coroutine coroutine;

    private const float StartingHorizontalScale = 1f;
    private const float CrouchingHorizontalScale = 0.6f;
    private const float StateChangeSpeed = 12f;

    private bool isMovementEnabled;
    private bool isCrouching;
    private bool isWalking;
    private bool isSprinting;


    private void Awake ()
    {
        isMovementEnabled = true;
        controller = GetComponent<CharacterController>();
        groundCheck = transform.GetChild(0).GetComponent<Transform>();
        groundMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Prop"));
    }

    public void Move (Vector3 moveVector)
    {
        if (!isMovementEnabled) return;
        moveVector = Quaternion.Euler(0, transform.eulerAngles.y, 0) * moveVector;
        controller.Move(moveVector * Time.deltaTime);
    }

    public void Crouch ()
    {
        if (coroutine != null){
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(ChangeCrouchingState());
    }
    
    public void Walk ()
    {
        // TODO
    }

    public void Run ()
    {
        // TODO
    }

    public void Sprint ()
    {
        // TODO
    }

    public void ChangeMovementType (MovementType movementType)
    {
        // TODO
        this.movementType = movementType;
    }

    public MovementType Movement {
        get { return movementType; }
    }

    public bool IsMovementEnabled {
        get { return isMovementEnabled; }
        set { isMovementEnabled = value; }
    }

    public bool IsGrounded {
        get { return controller.isGrounded || Physics.CheckSphere(groundCheck.position, .4f, groundMask); }
    }

    public bool IsCrouching {
        get { return isCrouching; }
        set { isCrouching = value; }
    }

    public bool IsSprinting {
        get { return isSprinting; }
        set { isSprinting = value; }
    }

    public bool IsWalking {
        get { return isWalking; }
        set { isWalking = value; }
    }

    #region Coroutines

    private IEnumerator ChangeCrouchingState () 
    {
        Vector3 currentScale = transform.localScale;
        if (isCrouching)
        {
            while (CrouchingHorizontalScale != currentScale.y)
            {

                if (CrouchingHorizontalScale >= currentScale.y - .05f)
                {
                    currentScale.y = CrouchingHorizontalScale;
                    transform.localScale = currentScale;
                }
                else
                {
                    currentScale.y = Mathf.Lerp(currentScale.y, CrouchingHorizontalScale, Time.deltaTime * StateChangeSpeed);
                    transform.localScale = currentScale;
                }

                yield return null;
            }
        }
        else 
        {
            while (StartingHorizontalScale != currentScale.y)
            {
                if (StartingHorizontalScale <= currentScale.y - .05f)
                {
                    currentScale.y = StartingHorizontalScale;
                    transform.localScale = currentScale;
                }
                else
                {
                    currentScale.y = Mathf.Lerp(currentScale.y, StartingHorizontalScale, Time.deltaTime * StateChangeSpeed);
                    transform.localScale = currentScale;
                }

                yield return null;
            }
        }
        coroutine = null;
    }

    #endregion
}

public enum MoveDirection{
    Forwards, Backwards, Left, Right
}

public enum MovementType {
    Walk, Run, Sprint
}
