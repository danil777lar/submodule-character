using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICursorCamera
{
    public int Priority { get; }
    
    public Vector3 DefaultDirection { get; }

    public Vector2 RotationLimitMin { get; }
    public Vector2 RotationLimitMax { get; }
}
