using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BigFan : MonoBehaviour
{
    public float windSpeed = 0.1f;
    [FormerlySerializedAs("windDirection")] public WindDirection WindDirection = WindDirection.Outward;
    [FormerlySerializedAs("fanDirection")] public FanDirection FanDirection = FanDirection.Up;
    public SpriteRenderer spriteRenderer;
    public Sprite spriteInward;
    public Sprite spriteOutward;
    public float maxRange = 99f;
    private WindDirection _initialWindDirection;
    
    private void Start()
    {
        spriteRenderer.sprite = WindDirection == WindDirection.Inward ? spriteInward : spriteOutward;
        _initialWindDirection = WindDirection;
    }
    
    public void SetSprite()
    {   
        spriteRenderer.sprite = WindDirection == WindDirection.Inward ? spriteInward : spriteOutward;
    }
    
    public void ResetwindDirection()
    {
        WindDirection = _initialWindDirection;
        SetSprite();
    }
}

public enum WindDirection
{
    Inward,
    Outward
}

public enum FanDirection
{
    Up,
    Down,
    Left,
    Right
}