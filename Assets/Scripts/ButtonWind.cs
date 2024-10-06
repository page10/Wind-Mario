using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonWind : MonoBehaviour
{
    public SpriteRenderer buttonSprite;
    public SpriteRenderer windSprite;
    
    public Sprite spriteInward;
    public Sprite spriteOutward;
    public Sprite spritePressed;
    public Sprite spriteNormal;
    public ButtonStatus Status { get; private set; } = ButtonStatus.Normal;
    public ButtonType Type = ButtonType.Inward;
    private List<Segment> segments = new List<Segment>();

    [SerializeField] private List<BigFan> controlledFans = new List<BigFan>();
    
    public void SetStatus(ButtonStatus status)
    {
        Status = status;
        switch (Status)
        {
            case ButtonStatus.Normal:
                buttonSprite.sprite = spriteNormal;
                break;
            case ButtonStatus.Pressed:
                buttonSprite.sprite = spritePressed;
                break;
        }
    }

    public List<BigFan> GetControlledFans()
    {
        return controlledFans;
    }
    
    public void SetSegments(List<Segment> segments)
    {
        this.segments = segments;
    }
    
    public List<Segment> GetSegments()
    {
        return segments;
    }

    private void Start()
    {
        buttonSprite.sprite = spriteNormal;
        windSprite.sprite  = ButtonType.Inward == Type ? spriteInward : spriteOutward;
    }
    
    public void PressButton()
    {
        SetStatus(ButtonStatus.Pressed);
    }
    public void ResetButton()
    {
        SetStatus(ButtonStatus.Normal);
    }
}

public enum ButtonStatus
{
    Normal,
    Pressed
}

public enum ButtonType
{
    Inward,
    Outward
}
