using UnityEngine;
using System.Collections;

namespace Katana.PhysicsUtil
{
    public static class Physics
    {
        const float Epsilon = 1.0e-6f;
        //public int HitPointsCircleAndSegment(Vector2 c0, float r, Vector2 s1, Vector2 s2, out Vector2 o1, out Vector2 o2)
        //{
        //    o1 = Vector2.zero;
        //    o2 = Vector2.zero;

        //    var v1 = c0 - s1;
        //    var v2 = s2 - s1;

        //    var A = v1.sqrMagnitude;
        //    var B = Vector3.Dot(v1, v2);
        //    var C = v2.sqrMagnitude;

        //    // 判別式
        //    var D = B - A * C;

        //    if (D < 0)
        //        return 0;

        //    if(D == 0)
        //    {
        //        if (A < Epsilon)
        //            return 0;

        //        var t = B / A;
        //        if (t < 0 || t > 1)
        //            return 0;

        //        o1 = s1 + v2 * t;
        //        return 1;
        //    }

        //    if()
        //    return 0;
        //}
     
    }

}
