using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil
{
    public static Vector3 RoundVec3(Vector3 val, int decimals)
    {
        float x = (float)System.Math.Round(val.x, decimals);
        float y = (float)System.Math.Round(val.y, decimals);
        float z = (float)System.Math.Round(val.z, decimals);
        return new Vector3(x, y, z);
    }
}
