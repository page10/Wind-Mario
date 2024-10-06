using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Cage : MonoBehaviour
{
    public SpriteRenderer cageSprite;
    public SpriteRenderer cageDoorSprite;
    public SpriteRenderer light1sprite;
    public SpriteRenderer light2sprite;
    public SpriteRenderer light3sprite;
    
    public Sprite cageOpen;
    public Sprite cageClose;
    [FormerlySerializedAs("light1On")] public Sprite lightOn;
    [FormerlySerializedAs("light1Off")] public Sprite lightOff;
    
    private bool isOpen = false;
    private int lightOnCount = 0;
    
    public void AddLightOnCount()
    {
        lightOnCount++;
        if (lightOnCount == 1)
        {
            light1sprite.sprite = lightOn;
        }
        else if (lightOnCount == 2)
        {
            light2sprite.sprite = lightOn;
        }
        else if (lightOnCount == 3)
        {
            light3sprite.sprite = lightOn;
        }
    }
    
    public void OpenCage()
    {
        isOpen = true;
        cageSprite.sprite = cageOpen;
        cageDoorSprite.enabled = false;
    }
    
    public void ResetCage()
    {
        isOpen = false;
        cageSprite.sprite = cageClose;
        cageDoorSprite.enabled = true;
        lightOnCount = 0;
        light1sprite.sprite = lightOff;
        light2sprite.sprite = lightOff;
        light3sprite.sprite = lightOff;
    }
    
}
