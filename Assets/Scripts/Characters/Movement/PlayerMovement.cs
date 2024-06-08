using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : CharacterMover
{
    [Header ("Movement Information")]
    private Dictionary<MoveDirection, bool> moveStatus;
    private Vector3 adjustedMovementVector;
    private Vector3 movementVector;
    private Vector3 jumpingHorizontalMovementVector;
    private float currentSpeed;
    private float speedMultiplier;
    private bool movementEnabled;
    private bool ignoreJump;

    [Header("Stamina Information")]
    [SerializeField] private float playerStamina;
    [SerializeField] private float playerStaminaUseMultiplier;
    private const float BaseJumpStaminaCost = 5f;

    [Header("Events")] 
    private Action onJump;
    private Action onSprint;

    private void Start ()
    {
        // Initializing variables
        moveStatus = new Dictionary<MoveDirection, bool>(4)
        {
            { MoveDirection.Forwards, false },
            { MoveDirection.Backwards, false },
            { MoveDirection.Left, false },
            { MoveDirection.Right, false }
        };

        currentSpeed = movementSpeed[(int)MovementType.Run];
        speedMultiplier = defaultSpeedMultiplier;
        movementEnabled = true;
        ignoreJump = false;

        var input = InputManager.GetInputHandle();

        // Forwards Movement
        input.AddListenerOnInputAction(ChangeForwardMovementState, "Move Forwards");

        // Backwards Movement
        input.AddListenerOnInputAction(ChangeBackwardsMovementState, "Move Backwards");

        // Left Movement
        input.AddListenerOnInputAction(ChangeLeftMovementState, "Move Left");

        // Right Movement
        input.AddListenerOnInputAction(ChangeRightMovementState, "Move Right");

        // Sprinting
        input.AddListenerOnInputAction(ChangeSprintingState, "Sprint");

        // Walking
        input.AddListenerOnInputAction(ChangeWalkingState, "Walk");

        // Crouching
        input.AddListenerOnInputAction(ChangeCrouchingState, "Crouch");

        // Jumping
        input.AddListenerOnInputAction(Jump, "Jump");
        
        PlayerManager.SubscribeToOnPlayerStaminaChange(UpdatePlayerCurrentStamina);
        PlayerManager.SubscribeToOnPlayerStaminaMultiplierChange(UpdatePlayerStaminaUseMultiplier);
    }
    
    private void FixedUpdate()
    {
        var horizontalMovement = movementVector.normalized * (currentSpeed * speedMultiplier);

        IsMoving = (horizontalMovement.magnitude != 0f);
        
        if (!IsGrounded)
        {
            if (ignoreJump) ignoreJump = false;
            var jumpingMovementMask = new Vector3(Mathf.Sign(jumpingHorizontalMovementVector.x), 0f, Mathf.Sign(jumpingHorizontalMovementVector.z)) - jumpingHorizontalMovementVector.normalized;
            horizontalMovement *= jumpMovementReduction;
            horizontalMovement.Scale(jumpingMovementMask);
            horizontalMovement += jumpingHorizontalMovementVector;
        }
        
        
        
        adjustedMovementVector.x = horizontalMovement.x;
        adjustedMovementVector.z = horizontalMovement.z;
        

        ApplyGravity();
        
        Move(adjustedMovementVector);
        
        if (IsMoving && IsSprinting && moveStatus[MoveDirection.Forwards]) onSprint.Invoke();
    }

    #region Movement

        private void Jump (ButtonState state)
        {
            Debug.Log("ignore jump: " + ignoreJump +"\nIs grounded: " + IsGrounded);
            if (state == ButtonState.Up
                || !movementEnabled
                || ignoreJump
                || !IsGrounded
                || IsJumping
                || !CanJump()) return;

            ignoreJump = true;
            jumpingHorizontalMovementVector = adjustedMovementVector;
            jumpingHorizontalMovementVector.y = 0f;
            adjustedMovementVector.y = Mathf.Sqrt(Physics.gravity.y * gravityMultiplier * -2f * jumpHeight);
            IsJumping = true;
            
            Jump();
            onJump?.Invoke();
        }

        private void ApplyGravity ()
        {
            if (IsGrounded && adjustedMovementVector.y < 0f)
            {
                adjustedMovementVector.y = -4f;
                IsJumping = false;
            }
            else
            {
                adjustedMovementVector.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }
        }

        private void ChangeForwardMovementState (ButtonState state)
        {
            if (!movementEnabled) return;
            movementVector += state == ButtonState.Down ? 
                Vector3.forward :
                Vector3.back;
            
            currentSpeed = state == ButtonState.Down && IsSprinting ? 
                movementSpeed[(int)MovementType.Sprint] :
                movementSpeed[(int)MovementType.Run];
            if (IsSprinting) currentSpeed = movementSpeed[(int)MovementType.Run];

            moveStatus[MoveDirection.Forwards] = !moveStatus[MoveDirection.Forwards];
        }

        private void ChangeBackwardsMovementState (ButtonState state)
        {
            if (!movementEnabled) return;
            movementVector += state == ButtonState.Down ? 
                Vector3.back :
                Vector3.forward;

            moveStatus[MoveDirection.Backwards] = !moveStatus[MoveDirection.Backwards];
        }

        private void ChangeLeftMovementState (ButtonState state)
        {
            if (!movementEnabled) return;
            movementVector += state == ButtonState.Down ? 
                Vector3.left :
                Vector3.right;

            moveStatus[MoveDirection.Left] = !moveStatus[MoveDirection.Left];
        }

        private void ChangeRightMovementState (ButtonState state)
        {
            if (!movementEnabled) return;
            movementVector += state == ButtonState.Down ? 
                Vector3.right :
                Vector3.left;

            moveStatus[MoveDirection.Right] = !moveStatus[MoveDirection.Right];
        }

    #endregion

    #region Movement Type

        public void EnableMovement()
        {
            movementEnabled = true;
            const ButtonState state = ButtonState.Down;
            
            if (InputManager.IsKeyDown("Move Backwards")) ChangeBackwardsMovementState(state);
            if (InputManager.IsKeyDown("Move Forwards")) ChangeForwardMovementState(state);
            if (InputManager.IsKeyDown("Move Left")) ChangeLeftMovementState(state);
            if (InputManager.IsKeyDown("Move Right")) ChangeRightMovementState(state);
            if (InputManager.IsKeyDown("Crouch")) ChangeCrouchingState(state);
            if (InputManager.IsKeyDown("Sprint")) ChangeSprintingState(state);
        }

        public void DisableMovement()
        {
            const ButtonState state = ButtonState.Down;
            
            if (moveStatus[MoveDirection.Right]) ChangeRightMovementState(state);
            if (moveStatus[MoveDirection.Left]) ChangeLeftMovementState(state);
            if (moveStatus[MoveDirection.Forwards]) ChangeForwardMovementState(state);
            if (moveStatus[MoveDirection.Backwards]) ChangeBackwardsMovementState(state);
            
            if (IsCrouching) ChangeCrouchingState(state);
            if (IsSprinting) ChangeSprintingState(state);
            
            movementEnabled = false;
        }

        private void ChangeCrouchingState (ButtonState state)
        {
            if (!movementEnabled) return;
            IsCrouching = state == ButtonState.Down;

            speedMultiplier = IsCrouching ? crouchingSpeedMultiplier : defaultSpeedMultiplier;

            if (IsCrouching && IsSprinting) ChangeRunningState();
            
            Crouch();
        }

        private void ChangeWalkingState (ButtonState state)
        {
            IsWalking = state == ButtonState.Down;

            if (IsWalking)
            {
                currentSpeed = movementSpeed[(int)MovementType.Walk];
                Walk();
            }
            else 
            {
                ChangeRunningState();
            }
        }

        private void ChangeRunningState ()
        {
            currentSpeed = movementSpeed[(int)MovementType.Run];
            Run();
        }

        private void ChangeSprintingState (ButtonState state)
        {
            if (!movementEnabled || IsCrouching) return;
            
            IsSprinting = state == ButtonState.Down;

            if (IsSprinting && CanSprint())
            {
                if (moveStatus[MoveDirection.Forwards]) currentSpeed = movementSpeed[(int)MovementType.Sprint];
                Sprint();
            }
            else 
            {
                ChangeRunningState();
            }
        }

    #endregion

    private void UpdatePlayerStaminaUseMultiplier(float multiplier) { playerStaminaUseMultiplier = multiplier; }

    private void UpdatePlayerCurrentStamina(float stamina)
    {
        playerStamina = stamina;
        if (stamina == 0f && IsSprinting) ChangeSprintingState(ButtonState.Up);
    }

    private bool CanJump() { return playerStamina >= BaseJumpStaminaCost * playerStaminaUseMultiplier; }

    private bool CanSprint() { return playerStamina != 0f; }
    
    public void AddOnJumpListener(Action listener) { onJump += listener; }
    
    public void AddOnSprintListener(Action listener) { onSprint += listener; }
}
