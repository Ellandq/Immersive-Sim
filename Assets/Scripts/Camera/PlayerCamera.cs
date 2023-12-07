using UnityEngine;


public class PlayerCamera : MonoBehaviour, ICameraController
{
    public bool Enabled { get; set; }

    protected float sensitivity;

    protected float xRotation;
    
    protected Transform playerBody;

    public void Initialize(float sensitivity, Transform playerBody)
    {
        this.sensitivity = sensitivity;
        this.playerBody = playerBody;
    }
    public virtual void Move()
    {
        
    }

    public virtual Camera GetCamera()
    {
        return null;
    }
}
