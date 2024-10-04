using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏世界管理
/// </summary>
public class GameScene : MonoBehaviour
{
    private readonly List<SceneCollider> _colliders = new List<SceneCollider>();
    private readonly List<Character> _characters = new List<Character>();

    // 所有的天花板、地板等，在赋值完成时最好重新算一遍
    private List<Segment> _roofs = new List<Segment>();
    private List<Segment> _leftWalls = new List<Segment>();
    private List<Segment> _rightWalls = new List<Segment>();
    private List<Segment> _floors = new List<Segment>();

    /// <summary>
    /// 因为Start和FixedUpdate等时序问题
    /// </summary>
    private bool _gameRunning = false;

    private void Start()
    {
        //游戏开始的时候从场景拿到所有配置好的collider
        _colliders.Clear();
        SceneCollider[] colliders = GameObject.FindObjectsOfType<SceneCollider>();
        foreach (SceneCollider c in colliders)
        {
            Debug.Log("[" +_colliders.Count+"]"+c.gameObject.name + " Found");
            _colliders.Add(c);
        }
        RecheckMapSegments();
        
        //找到所有配好的角色
        _characters.Clear();
        Character[] characters = GameObject.FindObjectsOfType<Character>();
        foreach (Character c in characters)
        {
            Debug.Log("[" +_characters.Count+"]"+c.gameObject.name + " Found");
            _characters.Add(c);
        }

        //最后开始运行游戏逻辑
        _gameRunning = true;
    }

    private void FixedUpdate()
    {
        if (!_gameRunning) return;

        foreach (Character cha in _characters)
        {
            //这样得到了这个角色是不是玩家控制的
            bool underPlayerControl = cha.playerControl;
            //todo 如何让玩家操作的角色动起来呢？仔细考虑一下
            CharacterHorizonMove move = CharacterHorizonMove.None;
            bool doJump = false;
            if (underPlayerControl)
            {
                //todo 获得操作，然后给move和doJump赋值。
            }
            else
            {
                //todo 根据ai得到操作，给move和doJump赋值。
            }
            MoveCharacter(cha, move, doJump);
        }
    }


    /// <summary>
    /// 地图变化了，重新计算所有的墙壁等
    /// </summary>
    private void RecheckMapSegments()
    {
        _roofs.Clear();
        _leftWalls.Clear();
        _rightWalls.Clear();
        _floors.Clear();
        
        foreach (SceneCollider c in _colliders)
        {
            if (c.isFloor) _floors.Add(new Segment(new Vector2(c.Left, c.Top), new Vector2(c.Right, c.Top)));
            if (c.isRoof) _roofs.Add(new Segment(new Vector2(c.Left, c.Bottom), new Vector2(c.Right, c.Bottom)));
            if (c.isWall)
            {
                _leftWalls.Add(new Segment(new Vector2(c.Left, c.Top), new Vector2(c.Left, c.Bottom)));
                _rightWalls.Add(new Segment(new Vector2(c.Right, c.Top), new Vector2(c.Right, c.Bottom)));
            }
        }
        
        //todo 现在只是单纯的加入列表，看看每个方向是否有办法把一些原本就相连的线条合并掉，比如(0,0)->(0,1)和(0,1)->(0,2)应该合并为(0,0)->(0,2)
    }

    /// <summary>
    /// 单个角色的行动（每一帧使用）
    /// </summary>
    /// <param name="cha"></param>
    /// <param name="dir"></param>
    /// <param name="doJump"></param>
    private void MoveCharacter(Character cha, CharacterHorizonMove dir, bool doJump)
    {
        Vector2 chaPos = cha.transform.position;
        Vector2 wishToPos = new Vector2(
            cha.HorizontalMove(dir),
            cha.VerticalMove(doJump)
        );
        Vector2 finalMoveTo = wishToPos;
        bool finalOnGround = false;
        
        //角色本fixedUpdate的移动脚下中心“射线”
        Segment chaMove = new Segment(chaPos, wishToPos);

        //本fixedUpdate中角色脚左右2边点的“射线”
        Segment chaLeftFootMove = new Segment(cha.FootLeftPlus + chaPos, cha.FootLeftPlus + wishToPos); //也可以看做向左的左边下方点
        Segment chaRightFootMove = new Segment(cha.FootRightPlus + chaPos, cha.FootRightPlus + wishToPos); //也可以看做向右的右边下方点
        //上面3个点
        Segment chaHeadTop = new Segment(chaPos + cha.ColliderHeight, wishToPos + cha.ColliderHeight); //头顶点的位置变化“射线”
        Segment chaLeftHeadMove = new Segment(cha.FootLeftPlus + chaPos + cha.ColliderHeight, cha.FootLeftPlus + wishToPos + cha.ColliderHeight); //也可以看做向左的左边上方点
        Segment chaRightHeadMove = new Segment(cha.FootRightPlus + chaPos + cha.ColliderHeight, cha.FootRightPlus + wishToPos + cha.ColliderHeight); //也可以看做向右的右边上方点
        
        bool rising = wishToPos.y > chaPos.y; //是否在上升
        bool falling = wishToPos.y < chaPos.y; //是否在下落
        
        //todo 如果上升中（rising），就要获得是否撞到天花板了（当然是最接近自己脑袋的天花板），所有的天花板线_roof
        
        //todo 如果即飞上升，又非下落，就要判断脚下是否空了，空了就要下落（改变falling）
        
        //如果下落中（falling），就要判断是否落地了（最接近的地面就是落地）
        if (falling)
        {
            //往下掉的起点y
            float startY = chaPos.y;
            //最接近起点的y点坐标
            float nearestY = wishToPos.y;
            //遍历每个地板，与3条下落的射线，有交点，且最高的点，就是要落到的地面
            Segment[] check = new Segment[] { chaMove, chaLeftFootMove, chaRightFootMove };
            foreach (Segment floor in _floors)
            {
                foreach (Segment moveSeg in check)
                {
                    //如果碰到了地面，则会有移动线段和地板的交点
                    if (Geometry.SegmentIntersecting(moveSeg, floor, out Vector2 floorPoint))
                    {
                        finalOnGround = true; //肯定算碰到地面了
                        nearestY = Mathf.Max(nearestY, floorPoint.y); //2个y取更高的那个，毕竟是从上而下 
                    }
                    if (floor.p1.y <= 0) Debug.Log("cha.y" + finalMoveTo.y + ">>" + chaMove.p0 + "->" + chaMove.p1 + " <> " + floor.p0 + "->" + floor.p1 + ") >>" + finalOnGround + ">>>" + floorPoint);
                }
                
            }
            //到此，移动结果的y坐标确定，并且最终是否落地确定了
            finalMoveTo = new Vector2(finalMoveTo.x, nearestY);
            
        }
        
        //todo 上面获得了最终落点y值，接下来就是根据左右移动，分别和_leftWalls（向右移动）和_rightWalls（向左移动）来得到新的x，当然不移动的话，就简单了对吧……
        
        //todo 最终改变角色的OnGround等状态，以及设置其正确坐标（事实上，上面这个过程我们都会在改变finalMoveTo)
        
        //最后设置新的pos
        cha.transform.position = finalMoveTo;
        cha.SetOnGround(finalOnGround);
    }
}
