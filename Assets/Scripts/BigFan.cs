using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BigFan : MonoBehaviour
{
    public float windSpeed = 0.1f;
    [FormerlySerializedAs("windDirection")] public WindDirection WindDirection = WindDirection.Outward;
    [FormerlySerializedAs("fanDirection")] public FanDirection FanDirection = FanDirection.Up;

    
    
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