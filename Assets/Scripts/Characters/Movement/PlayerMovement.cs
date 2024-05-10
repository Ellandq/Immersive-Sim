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
    private bool ignoreNextSprintInput;
    private bool ignoreNextWalkingInput;
    private bool ignoreNextJumpInput;
    private bool movementEnabled;

    [Header ("Movement settings")] 
    [SerializeField] private List<float> movementSpeed = new List<float>(){ 4f, 10f, 16f };
    [SerializeField] private float crouchingSpeedMultiplier = .4f;
    [SerializeField] private float defaultSpeedMultiplier = 1f;
    [SerializeField] private float jumpHeight = 1.8f;
    [SerializeField] private float jumpMovementReduction = 0.5f;
    [SerializeField] private float gravityMultiplier = 4f;

    [Header("Stamina Information")]
    [SerializeField] private float playerStamina;
    [SerializeField] private float playerStaminaUseMultiplier;
    private const float BaseJumpStaminaCost = 5f;

    [Header("Events")] 
    private Action onJump;
    private Action onSprint;

    private void Start ()
    {
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

        var input = InputManager.GetInputHandle();

        // Forwards Movement
        input.AddListenerOnButtonDown(ChangeForwardMovementState, "Move Forwards");
        input.AddListenerOnButtonUp(ChangeForwardMovementState, "Move Forwards");

        // Backwards Movement
        input.AddListenerOnButtonDown(ChangeBackwardsMovementState, "Move Backwards");
        input.AddListenerOnButtonUp(ChangeBackwardsMovementState, "Move Backwards");

        // Left Movement
        input.AddListenerOnButtonDown(ChangeLeftMovementState, "Move Left");
        input.AddListenerOnButtonUp(ChangeLeftMovementState, "Move Left");

        // Right Movement
        input.AddListenerOnButtonDown(ChangeRightMovementState, "Move Right");
        input.AddListenerOnButtonUp(ChangeRightMovementState, "Move Right");

        // Sprinting
        input.AddListenerOnButtonDown(ChangeSprintingState, "Sprint");
        input.AddListenerOnButtonUp(ChangeSprintingState, "Sprint");

        // Walking
        input.AddListenerOnButtonDown(ChangeWalkingState, "Walk");
        input.AddListenerOnButtonUp(ChangeWalkingState, "Walk");

        // Crouching
        input.AddListenerOnButtonDown(ChangeCrouchingState, "Crouch");
        input.AddListenerOnButtonUp(ChangeCrouchingState, "Crouch");

        // Jumping
        input.AddListenerOnButtonDown(Jump, "Jump");
        input.AddListenerOnButtonUp(Jump, "Jump");
        
        PlayerManager.SubscribeToOnPlayerStaminaChange(UpdatePlayerCurrentStamina);
        PlayerManager.SubscribeToOnPlayerStaminaMultiplierChange(UpdatePlayerStaminaUseMultiplier);
    }
    
    private void Update()
    {
        var horizontalMovement = movementVector.normalized * (currentSpeed * speedMultiplier);

        IsMoving = (horizontalMovement.magnitude != 0f);
        
        if (!IsGrounded)
        {
            var jumpingMovementMask = new Vector3(Mathf.Sign(jumpingHorizontalMovementVector.x), 0f, Mathf.Sign(jumpingHorizontalMovementVector.z)) - jumpingHorizontalMovementVector.normalized;
            horizontalMovement *= jumpMovementReduction;
            horizontalMovement.Scale(jumpingMovementMask);
            horizontalMovement += jumpingHorizontalMovementVector;
        }
        
        adjustedMovementVector.x = horizontalMovement.x;
        adjustedMovementVector.z = horizontalMovement.z;
        

        ApplyGravity();
        
        Move(adjustedMovementVector);
        
        if (IsMoving && IsSprinting) onSprint.Invoke();
    }

    #region Movement

        private void Jump ()
        {
            if (!movementEnabled) return;
            if (ignoreNextJumpInput || !CanJump()){
                ignoreNextJumpInput = false;
                return;
            }
            ignoreNextJumpInput = true;
            if (!IsGrounded) return;
            jumpingHorizontalMovementVector = adjustedMovementVector;
            jumpingHorizontalMovementVector.y = 0f;
            adjustedMovementVector.y = Mathf.Sqrt(Physics.gravity.y * gravityMultiplier * -2f * jumpHeight);
            IsJumping = true;
            
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

        private void ChangeForwardMovementState ()
        {
            if (!movementEnabled) return;
            if (moveStatus[MoveDirection.Forwards]) 
            {
                movementVector -= Vector3.forward;
            } 
            else 
            {
                movementVector += Vector3.forward;
            }

            moveStatus[MoveDirection.Forwards] = !moveStatus[MoveDirection.Forwards];
        }

        private void ChangeBackwardsMovementState ()
        {
            if (!movementEnabled) return;
            if (moveStatus[MoveDirection.Backwards]) 
            {
                movementVector -= Vector3.back;
            } 
            else 
            {
                movementVector += Vector3.back;
            }

            moveStatus[MoveDirection.Backwards] = !moveStatus[MoveDirection.Backwards];
        }

        private void ChangeLeftMovementState ()
        {
            if (!movementEnabled) return;
            if (moveStatus[MoveDirection.Left]) 
            {
                movementVector -= Vector3.left;
            } 
            else 
            {
                movementVector += Vector3.left;
            }

            moveStatus[MoveDirection.Left] = !moveStatus[MoveDirection.Left];
        }

        private void ChangeRightMovementState ()
        {
            if (!movementEnabled) return;
            if (moveStatus[MoveDirection.Right]) 
            {
                movementVector -= Vector3.right;
            } 
            else 
            {
                movementVector += Vector3.right;
            }

            moveStatus[MoveDirection.Right] = !moveStatus[MoveDirection.Right];
        }

    #endregion

    #region Movement Type

        public void EnableMovement()
        {
            movementEnabled = true;
            
            if (InputManager.IsKeyDown("Move Backwards")) ChangeBackwardsMovementState();
            if (InputManager.IsKeyDown("Move Forwards")) ChangeForwardMovementState();
            if (InputManager.IsKeyDown("Move Left")) ChangeLeftMovementState();
            if (InputManager.IsKeyDown("Move Right")) ChangeRightMovementState();
            
            if (InputManager.IsKeyDown("Crouch")) ChangeCrouchingState();
            if (InputManager.IsKeyDown("Sprint")) ChangeSprintingState();
            if (InputManager.IsKeyDown("Jump")) ignoreNextJumpInput = true;
        }

        public void DisableMovement()
        {
            
            if (moveStatus[MoveDirection.Right]) ChangeRightMovementState();
            if (moveStatus[MoveDirection.Left]) ChangeLeftMovementState();
            if (moveStatus[MoveDirection.Forwards]) ChangeForwardMovementState();
            if (moveStatus[MoveDirection.Backwards]) ChangeBackwardsMovementState();
            
            if (IsCrouching) ChangeCrouchingState();
            if (IsSprinting) ChangeSprintingState();
            
            if (InputManager.IsKeyDown("Jump")) ignoreNextJumpInput = false;
            
            movementEnabled = false;
        }

        private void ChangeCrouchingState ()
        {
            if (!movementEnabled) return;
            IsCrouching = !IsCrouching;

            speedMultiplier = IsCrouching ? crouchingSpeedMultiplier : defaultSpeedMultiplier;

            if (IsCrouching && IsSprinting)
            {
                ignoreNextSprintInput = true;
                ChangeRunningState();
            }
            
            Crouch();
        }

        private void ChangeWalkingState ()
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

        private void ChangeSprintingState ()
        {
            if (!movementEnabled) return;
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

            if (IsSprinting && CanSprint())
            {
                currentSpeed = movementSpeed[(int)MovementType.Sprint];
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
        if (stamina == 0f && IsSprinting) ChangeSprintingState();
    }

    private bool CanJump() { return playerStamina >= BaseJumpStaminaCost * playerStaminaUseMultiplier; }

    private bool CanSprint() { return playerStamina != 0f; }
    
    public void AddOnJumpListener(Action listener) { onJump += listener; }
    
    public void AddOnSprintListener(Action listener) { onSprint += listener; }
}
