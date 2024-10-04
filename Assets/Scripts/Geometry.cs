using System;
using UnityEngine;

/// <summary>
/// 线段
/// </summary>
[Serializable]
public struct Segment
{
    public Vector2 p0;
    public Vector2 p1;
    
    public Segment(Vector2 p0, Vector2 p1)
    {
        this.p0 = p0;
        this.p1 = p1;
    }
}

/// <summary>
/// 一些几何函数
/// </summary>
public static class Geometry
{
    /// <summary>
    /// 两向量叉乘
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    private static float CrossProduct(Vector2 v1, Vector2 v2)
    {
        return v1.x * v2.y - v1.y * v2.x;
    }
    
    /// <summary>
    /// 判断2个线段是否交叉
    /// </summary>
    /// <param name="a">线段a</param>
    /// <param name="b">线段b</param>
    /// <param name="intersectionPoint">交点</param>
    /// <returns></returns>
    public static bool SegmentIntersecting(Segment a, Segment b, out Vector2 intersectionPoint)
    {
        Vector2 a1 = a.p0;
        Vector2 a2 = a.p1;
        Vector2 b1 = b.p0;
        Vector2 b2 = b.p1;
        Vector2 ab = new Vector2(a2.x - a1.x, a2.y - a1.y);
        Vector2 cd = new Vector2(b2.x - b1.x, b2.y - b1.y);
        Vector2 ac = new Vector2(b1.x - a1.x, b1.y - a1.y);
        Vector2 ad = new Vector2(b2.x - a1.x, b2.y - a1.y);

        float crossProduct1 = CrossProduct(ab, ac);
        float crossProduct2 = CrossProduct(ab, ad);
        float crossProduct3 = CrossProduct(cd, ac);
        float crossProduct4 = CrossProduct(cd, ad);

        bool res = (crossProduct1 * crossProduct2 < 0) && (crossProduct3 * crossProduct4 < 0);
        intersectionPoint = Vector2.zero;
        if (res)
        {
            float denominator = CrossProduct(ab, cd);
            float t = CrossProduct(cd, ac) / denominator;
            intersectionPoint = new Vector2(a1.x + t * ab.x, a1.y + t * ab.y);
        }

        return res;
    }
}