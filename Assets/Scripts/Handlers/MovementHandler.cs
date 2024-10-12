using UnityEngine;
using System.Collections;

public class MovementHandler
{
    #region Variables
    private PlayerController _controller;
    private PlayerData _data => _controller.Data;

    //Dash
    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    private bool _isDashAttacking;

    //Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;
    #endregion

    public MovementHandler(PlayerController controller)
    {
        _controller = controller;
        _dashesLeft = _data.dashAmount;
    }

    public void UpdateMovement()
    {
        HandleJump();
        HandleDash();
        HandleSlide();
    }

    public void FixedUpdateMovement()
    {
        if (!_controller.IsDashing)
        {
            if (_controller.IsWallJumping)
                Run(_data.wallJumpRunLerp);
            else
                Run(1);
        }
        else if (_isDashAttacking)
        {
            Run(_data.dashEndRunLerp);
        }

        if (_controller.IsSliding)
            Slide();
    }

    private void HandleJump()
    {

        if (_controller.IsJumping && _controller.RB.velocity.y < 0)
        {
            _controller.IsJumping = false;
            _controller.IsJumpFalling = true;

            if (!_controller.IsWallJumping)
                _controller.IsJumpFalling = true;
        }

        if (_controller.IsWallJumping && Time.time - _wallJumpStartTime > _controller.Data.wallJumpTime)
        {
            _controller.IsWallJumping = false;
        }

        if (_controller.LastOnGroundTime > 0 && !_controller.IsJumping && !_controller.IsWallJumping)
        {
            _controller.IsJumpCut = false;
            _controller.IsJumpFalling = false;

            if (!_controller.IsJumping)
                _controller.IsJumpFalling = false;
        }

        if (!_controller.IsDashing)
        {
            //jump
            if (CanJump() && _controller.LastPressedJumpTime > 0)
            {
                Jump();
                _controller.IsJumping = true;
                _controller.IsWallJumping = false;
                _controller.IsJumpCut = false;
                _controller.IsJumpFalling = false;
                Jump();
            }

            //wall Jump
            else if (CanWallJump() && _controller.LastPressedJumpTime > 0)
            {
                _controller.IsWallJumping = true;
                _controller.IsJumping = false;
                _controller.IsJumpCut = false;
                _controller.IsJumpFalling = false;

                _wallJumpStartTime = Time.time;
                _lastWallJumpDir = (_controller.LastOnWallRightTime > 0) ? -1 : 1;

                WallJump(_lastWallJumpDir);
                _controller.IsWallJumping = true;
            }
        }
    }

    private void HandleDash()
    {
        if (CanDash() && _controller.LastPressedDashTime > 0)
        {
            //Freeze game for split second for effect and forgiveness over directional input
            _controller.Sleep(_data.dashSleepTime);

            //If no direction pressed dashed foward in current direction
            if (_controller.MoveInput != Vector2.zero)
                _lastDashDir = _controller.MoveInput;
            else
                _lastDashDir = _controller.IsFacingRight ? Vector2.right : Vector2.left;

            _controller.IsDashing = true;
            _controller.IsJumping = false;
            _controller.IsWallJumping = false;
            _controller.IsJumpCut = false;

            StartDash(_lastDashDir);
        }
    }

    private void HandleSlide()
    {
        _controller.IsSliding = CanSlide() &&
            ((_controller.LastOnWallLeftTime > 0 && _controller.MoveInput.x < 0) ||
             (_controller.LastOnWallRightTime > 0 && _controller.MoveInput.x > 0));
    }


    private void Run(float lerpAmount)
    {
        float targetSpeed = _controller.MoveInput.x * _data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(_controller.RB.velocity.x, targetSpeed, lerpAmount);

        float accelRate;
        if (_controller.LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount : _data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount * _data.accelInAir : _data.runDeccelAmount * _data.deccelInAir;

        if ((_controller.IsJumping || _controller.IsWallJumping || _controller.IsJumpFalling) && Mathf.Abs(_controller.RB.velocity.y) < _data.jumpHangTimeThreshold)
        {
            accelRate *= _data.jumpHangAccelerationMult;
            targetSpeed *= _data.jumpHangMaxSpeedMult;
        }

        if (_data.doConserveMomentum && Mathf.Abs(_controller.RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(_controller.RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && _controller.LastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        float speedDif = targetSpeed - _controller.RB.velocity.x;
        float movement = speedDif * accelRate;

        _controller.RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Slide()
    {
        float speedDif = _data.slideSpeed - _controller.RB.velocity.y;
        float movement = speedDif * _data.slideAccel;
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        _controller.RB.AddForce(movement * Vector2.up);
    }

    public void Jump()
    {
        _controller.LastPressedJumpTime = 0;
        _controller.LastOnGroundTime = 0;

        float force = _data.jumpForce;
        if (_controller.RB.velocity.y < 0)
            force -= _controller.RB.velocity.y;

        _controller.RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }


    public void WallJump(int dir)
    {
        _controller.LastPressedJumpTime = 0;
        _controller.LastOnGroundTime = 0;
        _controller.LastOnWallRightTime = 0;
        _controller.LastOnWallLeftTime = 0;

        Vector2 force = new Vector2(_data.wallJumpForce.x, _data.wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(_controller.RB.velocity.x) != Mathf.Sign(force.x))
            force.x -= _controller.RB.velocity.x;

        if (_controller.RB.velocity.y < 0)
            force.y -= _controller.RB.velocity.y;

        _controller.RB.AddForce(force, ForceMode2D.Impulse);
    }

    public void StartDash(Vector2 dir)
    {
        _controller.LastOnGroundTime = 0;
        _controller.LastPressedDashTime = 0;

        _dashesLeft--;
        _isDashAttacking = true;

        _controller.StartCoroutine(DashRoutine(dir));
    }


    private IEnumerator DashRoutine(Vector2 dir)
    {
        float startTime = Time.time;

        _controller.GravityHandler.SetGravityScale(0);

        while (Time.time - startTime <= _data.dashAttackTime)
        {
            _controller.RB.velocity = dir.normalized * _data.dashSpeed;
            yield return null;
        }

        startTime = Time.time;

        _isDashAttacking = false;

        _controller.GravityHandler.SetGravityScale(_data.gravityScale);
        _controller.RB.velocity = _data.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= _data.dashEndTime)
        {
            yield return null;
        }

        _controller.IsDashing = false;
    }

    public bool CanJump() => _controller.LastOnGroundTime > 0 && !_controller.IsJumping;
    
    public bool CanWallJump() => _controller.LastPressedJumpTime > 0 && _controller.LastOnWallTime > 0 && _controller.LastOnGroundTime <= 0 &&
       (!_controller.IsWallJumping ||
       (_controller.LastOnWallRightTime > 0 && _lastDashDir.x > 0) ||
       (_controller.LastOnWallLeftTime > 0 && _lastDashDir.x < 0));

    public bool CanJumpCut() => _controller.IsJumping && _controller.RB.velocity.y > 0;

    public bool CanWallJumpCut() => _controller.IsWallJumping && _controller.RB.velocity.y > 0;

    public bool CanDash()
    {
        if (!_controller.IsDashing && _dashesLeft < _controller.Data.dashAmount && _controller.LastOnGroundTime > 0 && !_dashRefilling)
        {
            //Sending to PlayerController for asynch execution
            _controller.RequestRefillDash(1);
        }
        return _dashesLeft > 0;
    }

    public bool CanSlide() => _controller.LastOnWallTime > 0 && !_controller.IsJumping && !_controller.IsWallJumping && !_controller.IsDashing && _controller.LastOnGroundTime <= 0;
    public bool IsDashAttacking => _isDashAttacking;

    public bool DashRefilling
    {
        get => _dashRefilling;
        set => _dashRefilling = value;

    }

    public int DashesLeft
    {
        get => _dashesLeft;
        set => _dashesLeft = value;
    }

    public IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
}

