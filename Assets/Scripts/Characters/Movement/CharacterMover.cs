using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    private CharacterController controller;
    private Transform groundCheck;
    private LayerMask groundMask;
    private MovementType movementType;

    private const float startingHorizontalScale = 1f;
    private const float crouchingHorizontalScale = 0.4f;

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

    public void Crouch (){
        // TODO
        if (isCrouching)
        {
            Vector3 scale = new Vector3(0f, crouchingHorizontalScale, 0f);
            transform.localScale = scale;
        }
        else
        {
            Vector3 scale = new Vector3(0f, startingHorizontalScale, 0f);
            transform.localScale = scale;
        }
    }
    
    public void Walk (){
        // TODO
    }

    public void Run (){
        // TODO
    }

    public void Sprint (){
        // TODO
    }

    public void ChangeMovementType (MovementType movementType){
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
        get { return controller.isGrounded || Physics.CheckSphere(groundCheck.position, 1f, groundMask); }
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
}

public enum MoveDirection{
    Forwards, Backwards, Left, Right
}

public enum MovementType {
    Walk, Run, Sprint
}
