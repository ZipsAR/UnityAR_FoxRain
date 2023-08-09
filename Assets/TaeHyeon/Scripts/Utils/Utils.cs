using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    
    /// <summary>
    /// Returns the position immediately
    /// before the distance beforeDistance
    /// when a straight line is drawn from startPoint to endPoint
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="beforeDistance"></param>
    /// <returns></returns>
    public static Vector3 GetPointBeforeDistance(Vector3 startPoint, Vector3 endPoint, float beforeDistance)
    {
        float distance = Vector3.Distance(startPoint, endPoint);
        float x = endPoint.x - (beforeDistance / distance) * (endPoint.x - startPoint.x);
        float y = startPoint.y;
        float z = endPoint.z - (beforeDistance / distance) * (endPoint.z - startPoint.z);

        return new Vector3(x, y, z);
    }
}
