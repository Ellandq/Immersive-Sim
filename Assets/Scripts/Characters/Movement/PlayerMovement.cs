using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMover
{
    [Header ("Movement Information")]
    private Dictionary<MoveDirection, bool> moveStatus;
    private Vector3 adjustedMovementVector;
    private Vector3 movementVector;
    private Vector3 jumpingHorizontalMovementVector;
    private float currentSpeed;
    private float speedMultiplier;
    private bool ignoreNextSprintInput;
    private bool ignoreNextWalkingInput;
    private bool ignoreNextJumpInput;

    [Header ("Movement settings")] 
    private readonly List<float> MovementSpeed = new List<float>(){ 4f, 10f, 16f };
    private const float CrouchingSpeedMultiplier = .4f;
    private const float DefaultSpeedMultiplier = 1f;
    private const float JumpHeight = 1.8f;
    private const float JumpMovementReduction = 0.5f;
    private const float GravityMultiplier = 4f;


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

        PlayerInput input = InputManager.GetInputHandle();

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
        Vector3 horizontalMovement = movementVector.normalized * (currentSpeed * speedMultiplier);
        
        if (!IsGrounded)
        {
            Vector3 jumpingMovementMask = new Vector3(1f, 0f, 1f) - jumpingHorizontalMovementVector.normalized;
            horizontalMovement *= JumpMovementReduction;
            horizontalMovement.Scale(jumpingMovementMask);
            horizontalMovement += jumpingHorizontalMovementVector;
        }
        
        adjustedMovementVector.x = horizontalMovement.x;
        adjustedMovementVector.z = horizontalMovement.z;
        

        ApplyGravity();
        
        Move(adjustedMovementVector);
    }

    #region Movement

    private void Jump ()
    {
        if (ignoreNextJumpInput){
            ignoreNextJumpInput = false;
            return;
        }
        ignoreNextJumpInput = true;
        if (IsGrounded)
        {
            jumpingHorizontalMovementVector = adjustedMovementVector;
            jumpingHorizontalMovementVector.y = 0f;
            adjustedMovementVector.y = Mathf.Sqrt(Physics.gravity.y * GravityMultiplier * -2f * JumpHeight);
            IsJumping = true;
        }
    }

    private void ApplyGravity ()
    {
        if (IsGrounded && adjustedMovementVector.y < 0f)
        {
            adjustedMovementVector.y = -2f;
            IsJumping = false;
        }
        else
        {
            adjustedMovementVector.y += Physics.gravity.y * GravityMultiplier * Time.deltaTime;
        }
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

        if (IsCrouching && IsSprinting)
        {
            ignoreNextSprintInput = true;
            ChangeRunningState();
        }
        
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
            ChangeRunningState();
        }
    }

    public void ChangeRunningState ()
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
            ChangeRunningState();
        }
    }

    #endregion
}
