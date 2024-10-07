using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindRegion
{
    public Vector2 WindDirectionV2 { get; private set; }
    public float WindSpeed { get; private set; }
    public Vector2 LeftTop { get; private set; }
    public Vector2 RightTop { get; private set; }
    public Vector2 LeftBottom { get; private set; }
    public Vector2 RightBottom { get; private set; }

    public Vector3 Center => Vector3.Lerp(LeftBottom, RightTop, 0.5f);
    public Vector3 Size => RightTop - LeftBottom;
    
    public WindRegion(Vector2 windDirectionV2, float windSpeed, Vector2 leftBottom, Vector2 rightTop)
    {
        WindDirectionV2 = windDirectionV2;
        WindSpeed = windSpeed;
        LeftTop = new Vector2(leftBottom.x, rightTop.y);
        RightTop = rightTop;
        LeftBottom = leftBottom;
        RightBottom = new Vector2(rightTop.x, leftBottom.y);
    }
    
    public void SetWindDirection(WindDirection 
        windDirectionEnum, FanDirection fanDirectionEnum)
    {
        switch (windDirectionEnum)
        {
            case global::WindDirection.Outward:
                switch (fanDirectionEnum)
                {
                    case FanDirection.Up:
                        WindDirectionV2 = Vector2.up;
                        break;
                    case FanDirection.Down:
                        WindDirectionV2 = Vector2.down;
                        break;
                    case FanDirection.Left:
                        WindDirectionV2 = Vector2.left;
                        break;
                    case FanDirection.Right:
                        WindDirectionV2 = Vector2.right;
                        break;
                }
                break;
            case global::WindDirection.Inward:
                switch (fanDirectionEnum)
                {
                    case FanDirection.Up:
                        WindDirectionV2 = Vector2.down;
                        break;
                    case FanDirection.Down:
                        WindDirectionV2 = Vector2.up;
                        break;
                    case FanDirection.Left:
                        WindDirectionV2 = Vector2.right;
                        break;
                    case FanDirection.Right:
                        WindDirectionV2 = Vector2.left;
                        break;
                }
                break;
        }
    }
    
    public void SetWindSpeed(float windSpeed)
    {
        WindSpeed = windSpeed;
    }
    
    public void SetLeftTop(Vector2 leftTop)
    {
        LeftTop = leftTop;
    }
    
    public void SetRightTop(Vector2 rightTop)
    {
        RightTop = rightTop;
    }
    
    public void SetLeftBottom(Vector2 leftBottom)
    {
        LeftBottom = leftBottom;
    }
    
    public void SetRightBottom(Vector2 rightBottom)
    {
        RightBottom = rightBottom;
    }

    public bool IsPointInside(Vector2 point)
    {
        return point.x >= LeftBottom.x && point.x <= RightTop.x && point.y >= LeftBottom.y && point.y <= RightTop.y;
    }
}
