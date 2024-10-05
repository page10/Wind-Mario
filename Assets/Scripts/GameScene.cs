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
    private List<BigFan> _fans = new List<BigFan>();
    private List<WindRegion> _windRegions = new List<WindRegion>();

    // 所有的天花板、地板等，在赋值完成时最好重新算一遍
    private List<Segment> _roofs = new List<Segment>();
    private List<Segment> _leftWalls = new List<Segment>();
    private List<Segment> _rightWalls = new List<Segment>();
    private List<Segment> _floors = new List<Segment>();
    


    /// <summary>
    /// 因为Start和FixedUpdate等时序问题
    /// </summary>
    private bool _gameRunning = false;
    private bool jump = false;

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
        
        //找到所有配好的风扇
        _fans.Clear();
        BigFan[] fans = GameObject.FindObjectsOfType<BigFan>();
        foreach (BigFan f in fans)
        {
            Debug.Log("[" +_fans.Count+"]"+f.gameObject.name + " Found");
            _fans.Add(f);
        }
        CalculateWindRegions();

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
                move = Input.GetAxis("Horizontal") < 0 ? CharacterHorizonMove.Left :
                    Input.GetAxis("Horizontal") > 0 ? CharacterHorizonMove.Right : CharacterHorizonMove.None;
                doJump = jump;
                jump = false;
                //print("do jump: " + doJump + ">>" + jump);
            }
            else
            {
                //todo 根据ai得到操作，给move和doJump赋值。
            }
            MoveCharacter(cha, move, doJump);
        }
        //print("movey: " + _characters[0].CurrentSpeed.y + ">> On ground: " + _characters[0].OnGround + ">> Falling: " + _characters[0].Falling);
    }
    
    private void CalculateWindRegions()  // todo  墙都漏风
    {
        foreach (BigFan fan in _fans)
        {
            SceneCollider fanCollider = fan.gameObject.GetComponent<SceneCollider>();
            if (fan.FanDirection == FanDirection.Up)
            {
                if (fan.WindDirection == WindDirection.Outward)
                {

                    List<Vector2> points = new List<Vector2>();
                    foreach (var roof in _roofs)
                    {
                        points.Add(roof.p0);
                        points.Add(roof.p1);
                    }
                    // sort points according to their x
                    points.Sort((p1, p2) => p1.x.CompareTo(p2.x));
                    // remove points that y < fanCollider.Top
                    points.RemoveAll(p => p.y < fanCollider.Top);
                    // remove points that x < fanCollider.Left or x > fanCollider.Right
                    points.RemoveAll(p => p.x < fanCollider.Left || p.x > fanCollider.Right);
                    // add fanCollider left and right sides
                    points.Add(new Vector2(fanCollider.Left, fanCollider.Bottom));
                    points.Add(new Vector2(fanCollider.Right, fanCollider.Bottom));
                    // sort points according to their x again
                    points.Sort((p1, p2) => p1.x.CompareTo(p2.x));
                    List<Vector2> pointsOnFan = new List<Vector2>();
                    
                    // map the points on fan
                    foreach (var point in points)
                    {
                        pointsOnFan.Add(new Vector2(point.x, fanCollider.Bottom));
                    }
                    pointsOnFan.Sort((p1, p2) => p1.x.CompareTo(p2.x));

                    List<Segment> heights = new List<Segment>();
                    // find nearest roof of each pointOnFan
                    for (int i = 0; i < pointsOnFan.Count - 1; i++)
                    {
                        float nearestY = 999f;  // count down
                        Vector2 tempMiddle = new Vector2(pointsOnFan[i].x, (pointsOnFan[i].y + pointsOnFan[i + 1].y) / 2);
                        Segment tempLongest = new Segment(tempMiddle, new Vector2(pointsOnFan[i].x, nearestY));
                        foreach (var roof in _roofs)
                        {
                            if (Geometry.SegmentIntersecting(tempLongest, roof, out Vector2 roofPoint))
                            {
                                // if (roof.p0.y > tempMiddle.y)
                                // {
                                    nearestY = Mathf.Min(nearestY, roof.p0.y);
                                    tempLongest = new Segment(tempMiddle, new Vector2(tempMiddle.x, nearestY));
                                // }
                            }
                        }
                        heights.Add(tempLongest);
                    }


                    // calculate wind regions
                    for (int i = 0; i < pointsOnFan.Count - 1; i++)
                    {
                        Vector2 BottomLeft = pointsOnFan[i];
                        Vector2 TopLeft = new Vector2(pointsOnFan[i].x, heights[i].p1.y);
                        Vector2 BottomRight = pointsOnFan[i + 1];
                        Vector2 TopRight = new Vector2(pointsOnFan[i + 1].x, heights[i].p1.y);
                        

                        
                        WindRegion newRegion = new WindRegion(Vector2.up, fan.windSpeed, TopLeft, TopRight, BottomLeft, BottomRight);
                        newRegion.SetWindDirection(fan.WindDirection, fan.FanDirection);
                        _windRegions.Add(newRegion);
                    }
                    
                }
                
                
            }
        }
    }
    
    

    private void Update()
    {
        jump = Input.GetKey(KeyCode.Space);
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

    // Merge consecutive segments
    // _floors = MergeSegments(_floors);
    // _roofs = MergeSegments(_roofs);
    // _leftWalls = MergeSegments(_leftWalls);
    // _rightWalls = MergeSegments(_rightWalls);
}

private List<Segment> MergeSegments(List<Segment> segments)
{
    // Sort the segments by their start point
    segments.Sort((a, b) => a.p0.x.CompareTo(b.p0.x));

    List<Segment> merged = new List<Segment>();

    for (int i = 0; i < segments.Count; i++)
    {
        // Start with the current segment
        Segment current = segments[i];

        // While the end of the current segment is the same as the start of the next segment
        while (i < segments.Count - 1 && current.p1 == segments[i + 1].p0)
        {
            // Extend the current segment to the end of the next segment
            current = new Segment(current.p0, segments[i + 1].p1);
            i++;
        }

        // Add the merged segment to the result
        merged.Add(current);
    }

    return merged;
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
        
        //todo 如果上升中（rising），就要获得是否撞到天花板了（当然是最接近自己脑袋的天花板），所有的天花板线_roof
        if (rising)
        {
            float nearestY = wishToPos.y;
            Segment[] check = new Segment[] { chaHeadTop, chaLeftHeadMove, chaRightHeadMove };
            foreach (Segment roof in _roofs)
            {
                foreach (Segment moveSeg in check)
                {
                    if (Geometry.SegmentIntersecting(moveSeg, roof, out Vector2 roofPoint))
                    {
                        finalOnGround = false;
                        nearestY = Mathf.Min(nearestY, roof.p0.y - cha.ColliderHeight.y - 0.001f);
                    }
                }
            }
            //到此，移动结果的y坐标确定，并且最终是否撞到天花板确定了
            finalMoveTo = new Vector2(finalMoveTo.x, nearestY);
        }
        //如果下落中（falling，非rising即falling），就要判断是否落地了（最接近的地面就是落地）
        else
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
                        nearestY = Mathf.Max(nearestY, floor.p0.y); //2个y取更高的那个，毕竟是从上而下 todo 因为out的交点不对，所以直接取地板的y （反正地板肯定水平）
                    }
                    // if (floor.p1.y <= 0 && moveSeg.p0.y >= floor.p1.y && moveSeg.p1.y <= floor.p1.y) 
                    //     Debug.Log("cha.y" + finalMoveTo.y + ">>" + moveSeg.p0 + "->" + moveSeg.p1 + " <> " + floor.p0 + "->" + floor.p1 + ") >>" + finalOnGround + ">>>" + floorPoint);
                }
                
            }
            //到此，移动结果的y坐标确定，并且最终是否落地确定了，如果碰到地面，则往上抬0.001米，避免不断下落（这也是经典的“偷1像素”，否则脚下和地面相等就会出现判定错误）
            finalMoveTo = new Vector2(finalMoveTo.x, nearestY + (finalOnGround ? 0.001f : 0));
            
        }
        
        bool isLeft = wishToPos.x <= chaPos.x; //是否向左移动
        bool isRight = wishToPos.x >= chaPos.x; //是否向右移动
        
        //todo 上面获得了最终落点y值，接下来就是根据左右移动，分别和_leftWalls（向右移动）和_rightWalls（向左移动）来得到新的x，当然不移动的话，就简单了对吧……
        if (isLeft)
        {
            Segment[] check = new Segment[] {chaLeftFootMove, chaLeftHeadMove };
            foreach (Segment wall in _rightWalls)
            {
                foreach (Segment moveSeg in check)
                {
                    if (Geometry.SegmentIntersecting(moveSeg, wall, out Vector2 wallPoint))
                    {
                        finalMoveTo = new Vector2(wall.p0.x - cha.FootLeftPlus.x + 0.001f, finalMoveTo.y);
                    }
                }
            }
        }
        else if (isRight)
        {
            Segment[] check = new Segment[] {chaRightFootMove, chaRightHeadMove };
            foreach (Segment wall in _leftWalls)
            {
                foreach (Segment moveSeg in check)
                {
                    if (Geometry.SegmentIntersecting(moveSeg, wall, out Vector2 wallPoint))
                    {
                        finalMoveTo = new Vector2(wall.p0.x - cha.FootRightPlus.x - 0.001f, finalMoveTo.y);
                    }
                }
            }
        }

        
        //最后设置新的pos
        cha.transform.position = finalMoveTo;
        cha.SetOnGround(finalOnGround);
    }
    
    private void OnDrawGizmos()
    {
        // Set the color of the gizmos
        Gizmos.color = Color.blue;

        foreach (WindRegion region in _windRegions)
        {
            // Draw a rectangle for each wind region
            Vector2 topLeft = region.LeftTop;
            Vector2 topRight = region.RightTop;
            Vector2 bottomLeft = region.LeftBottom;
            Vector2 bottomRight = region.RightBottom;

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}
