using System;
using Spine;
using Spine.Unity;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField, Tooltip("作为外边框的renderer")] private SpriteRenderer sprite;
    [SerializeField]private SkeletonAnimation image;
    private string _currentAction;
    private SkeletonData _imageData;
    private MeshRenderer _meshRenderer;
    // X轴控制变量
    [Tooltip("每个FixedUpdate左右方向移动加速度")] public float moveSpeed = 0.02f;
    [Tooltip("刹车速度，也就是每个FixedUpdate减少多少")] public float speedDown = 0.05f;
    [Tooltip("最大左右移动速度")] public float maxSpeed = 0.3f;
    [Tooltip("是否接受玩家操作")] public bool playerControl = true;

    [Tooltip("最小速度，小于这个速度就停止")] public float minSpeed = 0.01f;
    [Tooltip("空中减速系数")] public float airSpeedDown = 0.5f;
    // Y轴控制变量
    [Tooltip("下落时额外的重力加速度")] public float fallMultiplier = 3f;
    [Tooltip("每个fixedUpdate会下落的米数，也就是重力加速度")] public float gravity = 5f;
    [Tooltip("起跳的力量，按住跳每个fixed update都会往上这么多")] public float jumpForce = 5f;

    private bool _isJumping = false;
    
    public bool IsTouchingLeftWall { get; private set; } = false;
    public bool IsTouchingRightWall { get; private set; } = false;
    /// <summary>
    /// 当前是否在地面上
    /// </summary>
    public bool OnGround { get; private set; } = false;

    public void SetOnGround(bool on)
    {
        OnGround = on;
        //print("OnGround: " + on + ">>>" + CurrentSpeed.y);
        if (on) CurrentSpeed = new Vector2(CurrentSpeed.x, 0);
    }
    
    /// <summary>
    /// 当前这个FixedUpdate要移动的速度
    /// </summary>
    public Vector2 CurrentSpeed { get; private set; } = Vector2.zero;

    /// <summary>
    /// 判断角色是否下落中
    /// </summary>
    public bool Falling => !OnGround && CurrentSpeed.y < 0;
    
    //脚下左右两点的加值
    public Vector2 FootLeftPlus => Vector2.left * sprite.bounds.size.x / 2.00f;
    public Vector2 FootRightPlus => Vector2.right * sprite.bounds.size.x / 2.00f;
    //头上3点的y加值
    public Vector2 ColliderHeight => Vector2.up * sprite.bounds.size.y;

    /// <summary>
    /// 每一帧都要告诉我，往什么方向移动，我会返回我期望的水平移动的x值
    /// </summary>
    /// <param name="dir"></param>
    public float HorizontalMove(CharacterHorizonMove dir)
    {
        if ((IsTouchingLeftWall && dir == CharacterHorizonMove.Left) ||
        (IsTouchingRightWall && dir == CharacterHorizonMove.Right))
        {
            return transform.position.x;
        }
        
        float currentMoveSpeed = OnGround ? moveSpeed : moveSpeed * airSpeedDown;
        float currentSpeedDown = OnGround ? speedDown : speedDown * airSpeedDown;
        switch (dir)
        {
            case CharacterHorizonMove.Left:
                if (CurrentSpeed.x > 0 && OnGround) CurrentSpeed = Vector2.left * currentMoveSpeed;
                else if (CurrentSpeed.x <= -maxSpeed + currentMoveSpeed) CurrentSpeed = new Vector2(-maxSpeed, CurrentSpeed.y);
                else CurrentSpeed += Vector2.left * currentMoveSpeed;
                break;
            case CharacterHorizonMove.Right:
                if (CurrentSpeed.x < 0 && OnGround) CurrentSpeed = Vector2.right * currentMoveSpeed;
                else if (CurrentSpeed.x >= maxSpeed - currentMoveSpeed) CurrentSpeed = new Vector2(maxSpeed, CurrentSpeed.y);
                else CurrentSpeed += Vector2.right * currentMoveSpeed;
                break;
            case CharacterHorizonMove.None:
                float slowDownAmount = Mathf.Abs(CurrentSpeed.x) * currentSpeedDown;
                if (CurrentSpeed.x > minSpeed) CurrentSpeed = new Vector2(CurrentSpeed.x - slowDownAmount, CurrentSpeed.y);
                else if (CurrentSpeed.x < -minSpeed) CurrentSpeed = new Vector2(CurrentSpeed.x + slowDownAmount, CurrentSpeed.y);
                else if (Mathf.Abs(CurrentSpeed.x) < minSpeed) CurrentSpeed = new Vector2(0, CurrentSpeed.y);
                break;
        }
        return transform.position.x + CurrentSpeed.x;
    }

    /// <summary>
    /// 纵向移动，和HorizontalMove一起得到了这一帧期待的坐标点
    /// </summary>
    /// <param name="tryJumping">是否还在企图跳跃</param>
    /// <returns></returns>
    public float VerticalMove(bool tryJumping)
    {
        // //先加速度
        // OnGround = false;
        // // CurrentSpeed = new Vector2(CurrentSpeed.x, (tryJumping ? jump : 0) + CurrentSpeed.y - weight);
        // CurrentSpeed = new Vector2(CurrentSpeed.x, CurrentSpeed.y - weight);
        // bool canContinueJump = CurrentSpeed.y + jump >= 0;
        // return transform.position.y + CurrentSpeed.y + (tryJumping && canContinueJump ? jump : 0);
        float deltaTime = Time.fixedDeltaTime;
        
        if (OnGround && tryJumping && !_isJumping)
        {
            _isJumping = true;
            CurrentSpeed = new Vector2(CurrentSpeed.x, jumpForce);
            OnGround = false;
        }

        if (!OnGround)
        {
            float gravityMultiplier = (CurrentSpeed.y < 0 || !tryJumping) ? fallMultiplier : 1f;
            CurrentSpeed = new Vector2(CurrentSpeed.x, CurrentSpeed.y - gravity * gravityMultiplier * deltaTime);
        }
        
        float newPositionY = transform.position.y + CurrentSpeed.y * deltaTime;
        
        if (CurrentSpeed.y < 0) _isJumping = false;
        return newPositionY;
    }
    
    /// <summary>
    /// Set char speed to zero if the char is touching the wall
    /// </summary>
    /// <param name="touchingLeft"></param>
    /// <param name="touchingRight"></param>
    public void SetTouchingWall(bool touchingLeft, bool touchingRight)
    {
        IsTouchingLeftWall = touchingLeft;
        IsTouchingRightWall = touchingRight;
    }


    private void Start()
    {
        _imageData = image.SkeletonDataAsset.GetSkeletonData(true);
        _meshRenderer = image.GetComponent<MeshRenderer>();
        _meshRenderer.material = new Material(_meshRenderer.material);
    }
    
    public void ChangeAction(string action, bool loop = true)
    {
        if (_currentAction == action) return;
        Spine.Animation sa = _imageData?.FindAnimation(action);
        
        if (sa == null) return;
        Debug.Log("ChangeAction: " + action + ">>>" + sa);
        image.AnimationState.SetAnimation(0, action, loop);
        _currentAction = action;
    }
}

/// <summary>
/// 角色水平方向移动
/// </summary>
public enum CharacterHorizonMove
{
    None,
    Left,
    Right
}
