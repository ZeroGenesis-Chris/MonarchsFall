<<<<<<< HEAD
using System.Data.Common;
using UnityEngine;

public class GravityHandler
{
    private PlayerController _controller;
    private CoreMovement _coremovement;

    public GravityHandler(PlayerController controller, CoreMovement coremovement)
    {
        _controller = controller;
        _coremovement = coremovement;
    }

    public void UpdateGravity()
    {
        if (_coremovement.IsDashAttacking)
        {
            // No gravity while dashing
            SetGravityScale(0);
            return;
        }

        if (_controller.IsSliding)
        {
            SetGravityScale(0);
        }
        else if (_controller.RB.velocity.y < 0 && _controller.MoveInput.y < 0)
        {
            // Higher gravity if holding down
            SetGravityScale(_controller.Data.gravityScale * _controller.Data.fastFallGravityMult);
            _controller.RB.velocity = new Vector2(_controller.RB.velocity.x, Mathf.Max(_controller.RB.velocity.y, -_controller.Data.maxFastFallSpeed));
        }
        else if (_controller.IsJumpCut)
        {     
            // Higher gravity if jump button released
            SetGravityScale(_controller.Data.gravityScale * _controller.Data.jumpCutGravityMult);
            _controller.RB.velocity = new Vector2(_controller.RB.velocity.x, Mathf.Max(_controller.RB.velocity.y, -_controller.Data.maxFallSpeed));
        }
        else if ((_controller.IsJumping || _controller.IsWallJumping || _controller.IsJumpFalling) && Mathf.Abs(_controller.RB.velocity.y) < _controller.Data.jumpHangTimeThreshold)
        {

            SetGravityScale(_controller.Data.gravityScale * _controller.Data.jumpHangGravityMult);
        }
        else if (_controller.RB.velocity.y < 0)
        {
            // Higher gravity if falling
            SetGravityScale(_controller.Data.gravityScale * _controller.Data.fallGravityMult);
            _controller.RB.velocity = new Vector2(_controller.RB.velocity.x, Mathf.Max(_controller.RB.velocity.y, -_controller.Data.maxFallSpeed));
        }
        else
        {
            // Default gravity value
            SetGravityScale(_controller.Data.gravityScale);
        }
    }
    public void SetGravityScale(float scale)
    {
        _controller.RB.gravityScale = scale;
    }
}
=======
using UnityEngine;

public class CollisionHandler
{
    private readonly PlayerController _controller;
    private readonly Transform _groundCheckPoint;
    private readonly Vector2 _groundCheckSize;
    private readonly Transform _frontWallCheckPoint;
    private readonly Transform _backWallCheckPoint;
    private readonly Vector2 _wallCheckSize;
    private readonly LayerMask _groundLayer;

    public CollisionHandler(PlayerController controller, Transform groundCheckPoint, Vector2 groundCheckSize,
                            Transform frontWallCheckPoint, Transform backWallCheckPoint, Vector2 wallCheckSize,
                            LayerMask groundLayer)
    {
        _controller = controller;
        _groundCheckPoint = groundCheckPoint;
        _groundCheckSize = groundCheckSize;
        _frontWallCheckPoint = frontWallCheckPoint;
        _backWallCheckPoint = backWallCheckPoint;
        _wallCheckSize = wallCheckSize;
        _groundLayer = groundLayer;
    }

    public void CheckCollisions()
    {
        if (_controller.IsDashing || _controller.IsJumping)
        {
            return;
        }

        CheckGroundCollision();
        CheckWallCollision();
    }

    private void CheckGroundCollision()
    {
        bool isGrounded = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0f, _groundLayer);

        if (isGrounded)
        {
            _controller.LastOnGroundTime = _controller.Data.coyoteTime;
        }
    }

    private void CheckWallCollision()
    {
        bool isTouchingRightWall = IsTouchingWall(_controller.IsFacingRight);
        bool isTouchingLeftWall = IsTouchingWall(!_controller.IsFacingRight);

        if (isTouchingRightWall)
        {
            _controller.LastOnWallRightTime = _controller.Data.coyoteTime;
        }

        if (isTouchingLeftWall)
        {
            _controller.LastOnWallLeftTime = _controller.Data.coyoteTime;
        }

        _controller.LastOnWallTime = Mathf.Max(_controller.LastOnWallLeftTime, _controller.LastOnWallRightTime);
    }

    private bool IsTouchingWall(bool facingDirection)
    {
        if (facingDirection)
        {
            return Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0f, _groundLayer);
        }
        else
        {
            return Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0f, _groundLayer);
        }
    }
}
>>>>>>> master
