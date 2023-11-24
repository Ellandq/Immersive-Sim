using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    private CharacterController controller;
    private Transform groundCheck;
    private LayerMask groundMask;

    private bool isCrouching;
    private bool isSprinting;
    private bool isWalking;


    private void Awake ()
    {
        controller = GetComponent<CharacterController>();
        groundCheck = transform.GetChild(0).GetComponent<Transform>();
        groundMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Prop"));
    }

    public void Move (Vector3 moveVector)
    {
        controller.Move(moveVector * Time.deltaTime);
    }

    public void Crouch (){
        // TODO
    }

    public void Sprint (){

    }

    public void Walk (){

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
