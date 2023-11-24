using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    private CharacterController controller;
    private Transform groundCheck;
    private LayerMask groundMask;

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

    public bool isGrounded {
        get { return controller.isGrounded || Physics.CheckSphere(groundCheck.position, 1f, groundMask); }
    }
}
