using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    bool Enabled { get; set; }

    void Move();
    
    Camera GetCamera();
}
