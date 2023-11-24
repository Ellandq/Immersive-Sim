using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMover
{
    [Header ("Current Movement")]
    private Dictionary<MoveDirection, bool> moveStatus;
    private Vector3 adjustedMovementVector;
    private Vector3 movementVector;
    private float currentSpeed;
    private float speedMultiplier;
    private bool ignoreNextSprintInput;

    [Header ("Movement settings")]
    [SerializeField] private const float WalkingSpeed = 4f;
    [SerializeField] private const float RunningSpeed = 10f;
    [SerializeField] private const float SprintingSpeed = 16f;
    [SerializeField] private const float CrouchingSpeedMultiplier = .4f;
    [SerializeField] private const float DefaultSpeedMultiplier = 1f;


    private void Awake ()
    {
        currentSpeed = RunningSpeed;

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
    }
    
    private void Update()
    {
        
    }

    private void UpdateHorizontalMovement ()
    {
        
    }

    #region Movement

    private void ChangeForwardMovementState ()
    {
        Vector3 directionalMovement = new Vector3(0f, 0f, 1f) * currentSpeed;
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
        Vector3 directionalMovement = new Vector3(0f, 0f, -1f) * currentSpeed;
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
        Vector3 directionalMovement = new Vector3(-1f, 0f, 0f) * currentSpeed;
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
        Vector3 directionalMovement = new Vector3(1f, 0f, 0f) * currentSpeed;
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

    private void ChangeCrouchStatus ()
    {
        IsCrouching = !IsCrouching;
        if (IsCrouching)
        {
            speedMultiplier = CrouchingSpeedMultiplier;
        }
        else
        {
            speedMultiplier = DefaultSpeedMultiplier;
        }
        Crouch();
    }

    private void ChangeSprintStatus ()
    {
        IsSprinting = !IsSprinting;
        if (IsSprinting)
        {
            currentSpeed = SprintingSpeed;
        }
        else
        {
            currentSpeed = RunningSpeed;
        }
        Sprint();
    }

    private void ChangeWalkStatus ()
    {
        IsWalking = !IsWalking;
        if (IsWalking)
        {
            currentSpeed = WalkingSpeed;
        }
        else
        {
            currentSpeed = RunningSpeed;
        }
        Walk();
    }
}
