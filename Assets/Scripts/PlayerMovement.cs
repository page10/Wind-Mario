using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = 10f;
    public float jumpHeight = 5f;
    public float dashTimer = 0.15f;
    public float dashMultiplier = 10;
    public Transform groundCheckRayPoint, horizontalCheckRayPoint;
    public LayerMask groundLayer;
    
    private float horizontalSpeed;
    private float verticalSpeed;
    private float dashSpeed = 0;
    private bool isGrounded;
    private bool isWallAhead;
    private Vector2 movement;
    private float extraSpeed;
    private Vector2 rightScale, leftScale;
    private bool isDashing;
    private float fallMultiplier = 1;

    
    // Start is called before the first frame update
    void Start()
    {
        rightScale = leftScale = transform.localScale;
        leftScale.x *= -1;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        CheckJumping();
        GetInput();
        CheckFlip();
        ChechHorizontalCollision();
        CheckWallClimb();
        CheckDash();
        ApplyDash();
        ApplyMovement();
        
    }
    
    private void CheckGround()
    {
        RaycastHit2D _hit = Physics2D.Raycast(groundCheckRayPoint.position, -Vector2.up, 0.2f, groundLayer);
        if (_hit.collider)
        {
            // Debug.Log("Grounded");
            verticalSpeed = 0;
            isGrounded = true;
            fallMultiplier = 1;
        }
        else
        {
            verticalSpeed = Mathf.Lerp(verticalSpeed, -gravity * fallMultiplier, Time.deltaTime * 10);
        }
    }
    
    private void ChechHorizontalCollision()
    {
        RaycastHit2D _hit = Physics2D.Raycast(horizontalCheckRayPoint.position, Vector2.right * Mathf.Sign(transform.localScale.x), 0.2f);
        if (_hit.collider)
        {
            isWallAhead = true;
            transform.position -= new Vector3((0.2f -_hit.distance)* Mathf.Sign(transform.localScale.x), 0);
        }
        else
        {
            isWallAhead = false;
        }
    }
    
    private void CheckWallClimb()
    {
        RaycastHit2D _hit = Physics2D.Raycast(horizontalCheckRayPoint.position, Vector2.right * Mathf.Sign(transform.localScale.x), 0.5f);
        if (!isGrounded && verticalSpeed > 0 && _hit.collider && fallMultiplier >= 1)
        {
            verticalSpeed = 0;
            fallMultiplier = 0.01f;
        }
        else
        {
            fallMultiplier = 1f;
        }
    }
    
    private void CheckFlip()
    {
        if (horizontalSpeed > 0)
            transform.localScale = rightScale;
        else if (horizontalSpeed < 0)
            transform.localScale = leftScale;
    }
    
    private void CheckJumping()
    {
        if ((isGrounded || fallMultiplier < 1) && Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false;
            verticalSpeed = jumpHeight;
            
            fallMultiplier = 1;
        }
    }

    private void CheckDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            Invoke("EndDash", dashTimer);
        }
    }
    
    private void EndDash()
    {
        isDashing = false;
    }
    
    private void GetInput()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            extraSpeed = 2f;
        else
            extraSpeed = 1;

        horizontalSpeed = Input.GetAxis("Horizontal") * extraSpeed;
        
    }
    
    private void ApplyDash()
    {
        if (isDashing)
        {
            if (!isWallAhead)
                dashSpeed = Mathf.Lerp(dashSpeed, dashMultiplier * Mathf.Sign(transform.localScale.x), 10 * Time.deltaTime);
            else
            {
                isDashing = false;
                dashSpeed = 0;
            }
        }
        else
        {
            dashSpeed = Mathf.Lerp(dashSpeed, 0, 10 * Time.deltaTime);
        }
    }
    private void ApplyMovement()
    {
        if (isWallAhead)
            horizontalSpeed = 0;
        
        movement = new Vector2(horizontalSpeed + dashSpeed, verticalSpeed);
        transform.Translate(movement *moveSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(groundCheckRayPoint.position, -Vector2.up * 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(horizontalCheckRayPoint.position, Vector2.right * 0.2f * Mathf.Sign(transform.localScale.x));
    }
}
