using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFan : MonoBehaviour
{
    public float windSpeed = 0.1f;
    [SerializeField] private WindDirection windDirection = WindDirection.Outward;
    [SerializeField] private FanDirection fanDirection = FanDirection.Up;
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