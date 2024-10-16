using System.Data.Common;
using UnityEngine;

public class GravityHandler
{
    private PlayerController _controller;
    private MovementHandler _coremovement;

    public GravityHandler(PlayerController controller, MovementHandler coremovement)
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