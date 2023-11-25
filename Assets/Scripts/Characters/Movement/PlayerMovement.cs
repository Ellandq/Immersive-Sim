using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMover
{
    [Header ("Current Movement")]
    private Dictionary<MoveDirection, bool> moveStatus;
    [SerializeField] private Vector3 adjustedMovementVector;
    [SerializeField] private Vector3 movementVector;
    private float currentSpeed;
    private float speedMultiplier;
    private bool ignoreNextSprintInput;
    private bool ignoreNextWalkingInput;

    [Header ("Movement settings")]
    private readonly List<float> MovementSpeed = new List<float>(){ 4f, 10f, 16f };
    private const float CrouchingSpeedMultiplier = .4f;
    private const float DefaultSpeedMultiplier = 1f;
    private const float JumpHeight = 1.2f;


    private void Start ()
    {
        moveStatus = new Dictionary<MoveDirection, bool>(4)
        {
            { MoveDirection.Forwards, false },
            { MoveDirection.Backwards, false },
            { MoveDirection.Left, false },
            { MoveDirection.Right, false }
        };

        currentSpeed = MovementSpeed[(int)MovementType.Run];
        speedMultiplier = DefaultSpeedMultiplier;

        KeyboardInput input = InputManager.GetKeyboardHandle();

        // Forwards Movement
        input.onButtonDown["Move Forwards"] += ChangeForwardMovementState;
        input.onButtonUp["Move Forwards"] += ChangeForwardMovementState;

        // Backwards Movement
        input.onButtonDown["Move Backwards"] += ChangeBackwardsMovementState;
        input.onButtonUp["Move Backwards"] += ChangeBackwardsMovementState;

        // Left Movement
        input.onButtonDown["Move Left"] += ChangeLeftMovementState;
        input.onButtonUp["Move Left"] += ChangeLeftMovementState;

        // Right Movement
        input.onButtonDown["Move Right"] += ChangeRightMovementState;
        input.onButtonUp["Move Right"] += ChangeRightMovementState;

        // Sprinting
        input.onButtonDown["Sprint"] += ChangeSprintingState;
        input.onButtonUp["Sprint"] += ChangeSprintingState;

        // Walking
        input.onButtonDown["Walk"] += ChangeWalkingState;
        input.onButtonUp["Walk"] += ChangeWalkingState;

        // Crouching
        input.onButtonDown["Crouch"] += ChangeCrouchingState;
        input.onButtonUp["Crouch"] += ChangeCrouchingState;

        // Jumping
        input.onButtonDown["Jump"] += Jump;
        input.onButtonUp["Jump"] += Jump;
    }
    
    private void Update()
    {
        adjustedMovementVector = movementVector.normalized * currentSpeed * speedMultiplier;

        ApplyGravity();
        
        Move(adjustedMovementVector);
    }

    private void UpdateHorizontalMovement ()
    {
        
    }

    #region Movement

    private void Jump ()
    {
        adjustedMovementVector.y = Mathf.Sqrt(Physics.gravity.y * 2 * JumpHeight);
    }

    private void ChangeForwardMovementState ()
    {
        Vector3 directionalMovement = new Vector3(0f, 0f, 1f);
        if (moveStatus[MoveDirection.Forwards]) 
        {
            movementVector -= directionalMovement;
        } 
        else 
        {
            movementVector += directionalMovement;
        }

        moveStatus[MoveDirection.Forwards] = !moveStatus[MoveDirection.Forwards];
    }

    private void ChangeBackwardsMovementState ()
    {
        Vector3 directionalMovement = new Vector3(0f, 0f, -1f);
        if (moveStatus[MoveDirection.Backwards]) 
        {
            movementVector -= directionalMovement;
        } 
        else 
        {
            movementVector += directionalMovement;
        }

        moveStatus[MoveDirection.Backwards] = !moveStatus[MoveDirection.Backwards];
    }

    private void ChangeLeftMovementState ()
    {
        Vector3 directionalMovement = new Vector3(-1f, 0f, 0f);
        if (moveStatus[MoveDirection.Left]) 
        {
            movementVector -= directionalMovement;
        } 
        else 
        {
            movementVector += directionalMovement;
        }

        moveStatus[MoveDirection.Left] = !moveStatus[MoveDirection.Left];
    }

    private void ChangeRightMovementState ()
    {
        Vector3 directionalMovement = new Vector3(1f, 0f, 0f);
        if (moveStatus[MoveDirection.Right]) 
        {
            movementVector -= directionalMovement;
        } 
        else 
        {
            movementVector += directionalMovement;
        }

        moveStatus[MoveDirection.Right] = !moveStatus[MoveDirection.Right];
    }

    #endregion

    #region Movement Type 

    public void ChangeCrouchingState ()
    {
        IsCrouching = !IsCrouching;

        speedMultiplier = IsCrouching ? CrouchingSpeedMultiplier : DefaultSpeedMultiplier;
        
        Crouch();
    }

    public void ChangeWalkingState ()
    {
        if (ignoreNextWalkingInput)
        {
            ignoreNextWalkingInput = false;
            return;
        }

        if (IsSprinting)
        {
            ignoreNextWalkingInput = true;
            return;
        }

        IsWalking = !IsWalking;

        if (IsWalking)
        {
            currentSpeed = MovementSpeed[(int)MovementType.Walk];
            Walk();
        }
        else 
        {
            ChangeRuningState();
        }
    }

    public void ChangeRuningState ()
    {
        currentSpeed = MovementSpeed[(int)MovementType.Run];
        Run();
    }

    public void ChangeSprintingState ()
    {
        if (ignoreNextSprintInput)
        {
            ignoreNextSprintInput = false;
            return;
        }

        if (IsCrouching)
        {
            ignoreNextSprintInput = true;
            return;
        }

        IsSprinting = !IsSprinting;

        if (IsSprinting)
        {
            currentSpeed = MovementSpeed[(int)MovementType.Sprint];
            Sprint();
        }
        else 
        {
            ChangeRuningState();
        }
    }

    #endregion

    private void ApplyGravity ()
    {
        if (IsGrounded)
        {
            adjustedMovementVector.y = -2f;
        }
        else
        {
            adjustedMovementVector.y = Physics.gravity.y * Time.deltaTime;
        }
    }
}
