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
