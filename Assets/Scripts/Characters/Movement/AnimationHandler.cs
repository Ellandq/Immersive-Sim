using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private CharacterMover characterMover;
    
    public void JumpAddForce()
    {
        characterMover.JumpAddForce();
    }
}
