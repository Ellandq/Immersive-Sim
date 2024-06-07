using UnityEngine;

[System.Serializable]
public class PlayerCamera : MonoBehaviour, ICameraController
{
    public bool Enabled { get; set; }
    
    [SerializeField] protected Camera playerCamera;
    protected Rigidbody playerBody;
    
    protected float sensitivity;
    protected float xRotation;
    
    public void Initialize(float sensitivity, Rigidbody playerBody)
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
