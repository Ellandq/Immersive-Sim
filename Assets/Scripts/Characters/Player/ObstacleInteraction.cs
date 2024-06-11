using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleInteraction : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rigidbody = hit.collider.attachedRigidbody;

        if (!ReferenceEquals(rigidbody, null))
        {
            var pos = transform.position;
            var forceDirection = hit.gameObject.transform.position - pos;
            forceDirection.y = 0;
            forceDirection.Normalize();
            
            rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, pos, ForceMode.Impulse);
        }
    }
}
