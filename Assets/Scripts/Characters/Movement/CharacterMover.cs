using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheck;
    private Rigidbody characterRigidbody;

    [Header("Object Information")]
    private LayerMask groundMask;
    private Coroutine coroutine;

    [Header("Movement Settings")] 
    [SerializeField] protected List<float> movementSpeed = new List<float>(){ 5f, 10f, 16f };
    [SerializeField] protected float crouchingSpeedMultiplier = .4f;
    [SerializeField] protected float defaultSpeedMultiplier = 1f;
    [SerializeField] protected float jumpHeight = 1.8f;
    [SerializeField] protected float jumpMovementReduction = 0.8f;
    [SerializeField] protected float distanceFromGround = 1.6f;
    private Quaternion currentMovementAngle;

    [Header("Animation Info")] 
    private Vector2 animationState;
    private Vector2 targetAnimationState;
    private int xVelHash;
    private int yVelHash;
    private int zVelHash;
    private int groundHash;
    private int jumpHash;
    private int fallingHash;
    private int crouchHash;

    private void Awake ()
    {
        // Fetching Hash values from animator
        xVelHash = Animator.StringToHash("X_Velocity");
        yVelHash = Animator.StringToHash("Y_Velocity");
        zVelHash = Animator.StringToHash("Z_Velocity");
        jumpHash = Animator.StringToHash("Jump");
        groundHash = Animator.StringToHash("Grounded");
        fallingHash = Animator.StringToHash("Falling");
        crouchHash = Animator.StringToHash("Crouch");
        
        // Initializing variables
        groundMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Prop"));
        animationState = new Vector2();
        IsMovementEnabled = true;
        IsJumping = false;
        
        // Initializing components
        characterRigidbody = GetComponent<Rigidbody>();
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
            animator.SetFloat(yVelHash, characterRigidbody.velocity.y / 4);
        }
        
        // Adjusting current animation
        targetAnimationState = ConvertMovementVectorToAnimation(moveVector);
        animationState = Vector2.Lerp(animationState, targetAnimationState, Time.smoothDeltaTime * 2f);
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
        animator.SetBool(crouchHash, IsCrouching);
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
        IsJumping = true;
        animator.SetTrigger(jumpHash);
    }

    public void JumpAddForce()
    {
        if (IsGrounded)
        {
            characterRigidbody.AddForce(-characterRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            var jumpForce = Mathf.Sqrt(2f * jumpHeight * Physics.gravity.magnitude * characterRigidbody.mass) * 10;
            characterRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        animator.ResetTrigger(jumpHash);
    }

    private void SetAnimationGrounding(bool isGrounded)
    {
        animator.SetBool(fallingHash, !isGrounded);
        animator.SetBool(groundHash, isGrounded);
    }

    private void UpdateCurrentMovementAngle() { currentMovementAngle = Quaternion.Euler(0, transform.eulerAngles.y, 0); }

    protected bool IsGrounded
    {
        get
        {
            RaycastHit hitInfo;
            return Physics.Raycast(characterRigidbody.worldCenterOfMass, Vector3.down, out hitInfo,
                       distanceFromGround + .1f, groundMask)
                   || Physics.CheckSphere(groundCheck.position, 0.6f, groundMask);
        }
    }

    protected bool IsMoving { get; set; }
    
    protected bool IsJumping { get; set; }  

    protected bool IsCrouching { get; set; }

    protected bool IsSprinting { get; set; }

    protected bool IsWalking { get; set; }
    
    public bool IsMovementEnabled { get; set; }
    
    private static Vector2 ConvertMovementVectorToAnimation(Vector3 vector) { return new Vector2(vector.x / 5f, vector.z / 5f); }
    
    private void OnDrawGizmos()
    {
        if (characterRigidbody == null || groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(characterRigidbody.worldCenterOfMass, characterRigidbody.worldCenterOfMass + Vector3.down * (distanceFromGround + .1f));
        Gizmos.DrawWireSphere(groundCheck.position, 0.6f);
    }
}

public enum MoveDirection{
    Forwards, Backwards, Left, Right
}

public enum MovementType {
    Walk, Run, Sprint
}
