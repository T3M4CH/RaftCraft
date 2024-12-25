using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RotationHelper
{
    public static float FixAngle(this float angle)
    {
        return angle switch
        {
            > 180 => angle - 360,
            < -180 => angle + 360,
            _ => angle
        };
    }
}
