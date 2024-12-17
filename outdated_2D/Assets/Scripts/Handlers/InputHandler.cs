using System;
using UnityEngine;

/// <summary>
/// Handles player input and updates the <see cref="PlayerController"/> accordingly.
/// </summary>
public class InputHandler
{
    private PlayerController _controller;
    private IInputActions _inputActions;

    /// <summary>
    /// Gets the move input from the <see cref="IInputActions"/>.
    /// </summary>
    public Vector2 MoveInput => _inputActions.MoveInput;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputHandler"/> class.
    /// </summary>
    /// <param name="controller">The player controller to update.</param>
    /// <param name="inputActions">The input actions to handle.</param>
    public InputHandler(PlayerController controller, IInputActions inputActions)
    {
        _controller = controller;
        _inputActions = inputActions;
    }

    /// <summary>
    /// Handles the player input and updates the <see cref="PlayerController"/> accordingly.
    /// </summary>
    public void HandleInput()
    {
        // Handle Movement input
        Vector2 moveInput = _inputActions.MoveInput;

        // Update Facing direction based on input
        if (moveInput.x != 0)
        {
            _controller.SetFacingDirection(moveInput.x > 0);
        }

        // Handle Jump input
        if (_inputActions.JumpPressed)
        {
            _controller.LastPressedJumpTime = _controller.Data.jumpInputBufferTime;
            OnJump();
        }

        if (_inputActions.JumpReleased)
        {
            OnJumpCanceled();
        }

        // Handle Dash Inputs
        if (_inputActions.DashPressed)
        {
            _controller.LastPressedDashTime = _controller.Data.dashInputBufferTime;
            OnDash();
        }
    }

    /// <summary>
    /// Handles the jump input.
    /// </summary>
    private void OnJump()
    {
        if (_controller.CanJump())
        {
            _controller.Jump();
        }
        else if (_controller.CanWallJump())
        {
            int WallJumpDirection = _controller.LastOnWallRightTime > 0 ? -1 : 1;
            _controller.WallJump(WallJumpDirection);
        }
    }

    /// <summary>
    /// Handles the jump cancel input.
    /// </summary>
    private void OnJumpCanceled()
    {
       if (_controller.CanJumpCut() || _controller.CanWallJumpCut())
        {
        
            _controller.RB.AddForce(Vector2.down * _controller.RB.velocity.y * _controller.Data.jumpCutGravityMult, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Handles the dash input.
    /// </summary>
    private void OnDash()
    {
        if (_controller.CanDash())
        {
            Vector2 dashDirection = MoveInput != Vector2.zero ? MoveInput.normalized : (_controller.IsFacingRight ? Vector2.right : Vector2.left);
            _controller.StartDash(dashDirection);
        }
    }
}
