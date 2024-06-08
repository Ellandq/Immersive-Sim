using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Animator animator;
    private Rigidbody characterRigidbody;
    private Transform groundCheck;

    [Header("Object Information")]
    private LayerMask groundMask;
    private Coroutine coroutine;

    [Header("Movement Settings")] 
    [SerializeField] protected List<float> movementSpeed = new List<float>(){ 5f, 10f, 16f };
    [SerializeField] protected float crouchingSpeedMultiplier = .4f;
    [SerializeField] protected float defaultSpeedMultiplier = 1f;
    [SerializeField] protected float jumpHeight = 1.8f;
    [SerializeField] protected float jumpMovementReduction = 0.5f;
    [SerializeField] protected float gravityMultiplier = 4f;
    private Quaternion currentMovementAngle;

    [Header("Crouch Settings")]
    private const float StartingHorizontalScale = 1f;
    private const float CrouchingHorizontalScale = 0.6f;
    private const float StateChangeSpeed = 12f;

    [Header("Animation Info")] 
    private Vector2 animationState;
    private Vector2 targetAnimationState;
    private int xVelHash;
    private int yVelHash;
    private int zVelHash;
    private int groundHash;
    private int jumpHash;
    private int fallingHash;

    private void Awake ()
    {
        // Fetching Hash values from animator
        xVelHash = Animator.StringToHash("X_Velocity");
        yVelHash = Animator.StringToHash("Y_Velocity");
        zVelHash = Animator.StringToHash("Z_Velocity");
        jumpHash = Animator.StringToHash("Jump");
        groundHash = Animator.StringToHash("Grounded");
        fallingHash = Animator.StringToHash("Falling");
        
        // Initializing variables
        groundMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Prop"));
        animationState = new Vector2();
        IsMovementEnabled = true;
        IsJumping = false;
        
        // Initializing components
        characterRigidbody = GetComponent<Rigidbody>();
        groundCheck = transform.GetChild(0).GetComponent<Transform>();
        
    }

    protected void Move (Vector3 moveVector)
    {
        // If the player is grounded adjust the directional vector
        if (IsGrounded)
        {
            UpdateCurrentMovementAngle();
            SetAnimationGrounding(true);
        }
        else
        {
            SetAnimationGrounding(false);
            animator.SetFloat(zVelHash, characterRigidbody.velocity.y);
        }
        
        // Adjusting current animation
        targetAnimationState = ConvertMovementVectorToAnimation(moveVector);
        animationState = Vector2.Lerp(animationState, targetAnimationState, Time.deltaTime * 10f);
        animator.SetFloat(xVelHash, animationState.x);
        animator.SetFloat(zVelHash, animationState.y);
        
        // Calculating and executing the movement force
        moveVector = currentMovementAngle * moveVector;
        moveVector -= characterRigidbody.velocity;
        moveVector.y = 0f;
        characterRigidbody.AddForce(moveVector, ForceMode.VelocityChange);
    }

    protected void Crouch ()
    {
        // TODO Make crouching use animations rather the size changes
        if (coroutine != null){
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(ChangeCrouchingState());
    }
    
    protected void Walk ()
    {
        // TODO
    }

    protected void Run ()
    {
        // TODO
        if (IsSprinting) IsSprinting = false;
        if (IsWalking) IsWalking = false;
    }

    protected void Sprint ()
    {
        // TODO
    }

    protected void Jump()
    {
        animator.SetTrigger(jumpHash);
    }

    public void JumpAddForce()
    {
        characterRigidbody.AddForce(-characterRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
        characterRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        animator.ResetTrigger(jumpHash);
    }

    private void SetAnimationGrounding(bool isGrounded)
    {
        animator.SetBool(fallingHash, !isGrounded);
        animator.SetBool(groundHash, isGrounded);
    }

    private void UpdateCurrentMovementAngle()
    {
        currentMovementAngle = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    protected bool IsGrounded =>  Physics.CheckSphere(groundCheck.position, .7f, groundMask);
    
    protected bool IsMoving { get; set; }
    
    protected bool IsJumping { get; set; }

    protected bool IsCrouching { get; set; }

    protected bool IsSprinting { get; set; }

    protected bool IsWalking { get; set; }
    
    public bool IsMovementEnabled { get; set; }
    
    private static Vector2 ConvertMovementVectorToAnimation(Vector3 vector) { return new Vector2(vector.x / 5f, vector.z / 5f); }

    #region Coroutines

        // TODO REPLACE WITH ANIMATIONS 
        private IEnumerator ChangeCrouchingState () 
        {
            var currentScale = transform.localScale;
            if (IsCrouching)
            {
                while (CrouchingHorizontalScale != currentScale.y)
                {

                    if (CrouchingHorizontalScale >= currentScale.y - .05f)
                    {
                        currentScale.y = Mathf.MoveTowards(currentScale.y, CrouchingHorizontalScale, Time.deltaTime * StateChangeSpeed);
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
                        currentScale.y = Mathf.MoveTowards(currentScale.y, StartingHorizontalScale, Time.deltaTime * StateChangeSpeed);
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
