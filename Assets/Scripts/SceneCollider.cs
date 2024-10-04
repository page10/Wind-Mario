using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCollider : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [Tooltip("是否是地板，角色往下掉的时候可以站在地板上")] public bool isFloor;
    [Tooltip("是否是墙壁，角色左右移动会被墙壁卡")] public bool isWall;
    [Tooltip("是否是天花板，角色往上跳跃会顶到天花板而卡")] public bool isRoof;
    
    public float Left => sprite.bounds.min.x;
    public float Right => sprite.bounds.max.x;
    public float Top => sprite.bounds.max.y;
    public float Bottom => sprite.bounds.min.y;
}
