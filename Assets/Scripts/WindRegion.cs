using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindRegion
{
    public Vector2 WindDirection { get; private set; }
    public float WindSpeed { get; private set; }
    public Vector2 LeftTop { get; private set; }
    public Vector2 RightTop { get; private set; }
    public Vector2 LeftBottom { get; private set; }
    public Vector2 RightBottom { get; private set; }
    
    public WindRegion(Vector2 windDirection, float windSpeed, Vector2 leftTop, Vector2 rightTop, Vector2 leftBottom, Vector2 rightBottom)
    {
        WindDirection = windDirection;
        WindSpeed = windSpeed;
        LeftTop = leftTop;
        RightTop = rightTop;
        LeftBottom = leftBottom;
        RightBottom = rightBottom;
    }
    
    public void SetWindDirection(Vector2 windDirection)
    {
        WindDirection = windDirection;
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
}
