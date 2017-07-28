using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils {
    public static float NormalizeAngle(int rotation)
    {
        while (rotation < 0)
        {
            rotation += 360;
        }
        while (rotation > 360)
        {
            rotation -= 360;
        }
        return rotation;
    }

    public static Vector3 RoundVec3ToInt(Vector3 v)
    {
        return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }
}
