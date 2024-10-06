using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool isHit = false;
    public bool isHurt = false;
    public int hitCoolDown = 20;
    public Bounds Hitbox => spriteRenderer.bounds;

    public Dictionary<HitBox, int> HitRecord { get; private set; } = new Dictionary<HitBox, int>();
    
    public bool CanHit(HitBox hitBox) => !HitRecord.ContainsKey(hitBox) || HitRecord[hitBox] <= 0;  // 我没有命中过这个盒子或者命中过 但是时间已经过了
    

    public int attack;
    public int hp;
    
    public bool Dead => hp <= 0;
    
    public void AddHitRecord(HitBox hitBox, int time)
    {
        if (!HitRecord.TryAdd(hitBox, time)) HitRecord[hitBox] = time;


    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawCube(Hitbox.center, Hitbox.size);
    // }
}
