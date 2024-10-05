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
    /// todo 显然这个算法有问题，交点是不对的
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
        
        Vector2 ab = a2 - a1;
        Vector2 ac = b1 - a1;
        Vector2 ad = b2 - a1;

        float cross1 = CrossProduct(ab, ac);
        float cross2 = CrossProduct(ab, ad);
        
        Vector2 cd = b2 - b1;
        Vector2 ca = a1 - b1;
        Vector2 cb = a2 - b1;

        // 计算向量 CD 和 CA 的叉乘结果
        float cross3 = CrossProduct(cd, ca);
        // 计算向量 CD 和 CB 的叉乘结果
        float cross4 = CrossProduct(cd, cb);

        // 如果 AB 与 AC、AD 的叉乘结果符号不同，且 CD 与 CA、CB 的叉乘结果符号不同，则两条线段相交
        bool res = cross1 * cross2 < 0 && cross3 * cross4 < 0;

        intersectionPoint = Vector2.zero;
        if (res)
        {
            float denominator = CrossProduct(ab, cd);
            float t = CrossProduct(cd, ac) / denominator;
            intersectionPoint = new Vector2(a1.x + t * ab.x, a1.y + t * ab.y);
        }

        return res;
    }
    
    public static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float denominator = (b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y);
        float a1 = ((b.y - c.y) * (p.x - c.x) + (c.x - b.x) * (p.y - c.y)) / denominator;
        float a2 = ((c.y - a.y) * (p.x - c.x) + (a.x - c.x) * (p.y - c.y)) / denominator;
        float a3 = 1 - a1 - a2;
        return a1 >= 0 && a1 <= 1 && a2 >= 0 && a2 <= 1 && a3 >= 0 && a3 <= 1;
    }
    
    public static bool PointInRectangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        return PointInTriangle(p, a, b, c) || PointInTriangle(p, a, c, d);
    }
    
    public static bool PointInRectangle(Vector2 p, float left, float right, float top, float bottom)
    {
        return p.x >= left && p.x <= right && p.y >= bottom && p.y <= top;
    }
}