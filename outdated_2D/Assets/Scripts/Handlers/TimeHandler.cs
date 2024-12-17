using UnityEngine;

public class TimerHandler
{
    private PlayerController _controller;

    // List of timers to update
    private float[] timers;

    public TimerHandler(PlayerController controller)
    {
        _controller = controller;

        // Initialize the timer array with references to the controller's timers
        timers = new float[]
        {
            _controller.LastOnGroundTime,
            _controller.LastOnWallTime,
            _controller.LastOnWallRightTime,
            _controller.LastOnWallLeftTime,
            _controller.LastPressedJumpTime,
            _controller.LastPressedDashTime
        };
    }

    public void UpdateTimers()
    {
        for (int i = 0; i < timers.Length; i++)
        {
            timers[i] = Mathf.Max(timers[i] - Time.deltaTime, 0);
        }

        // Assign the values back to the controller
        _controller.LastOnGroundTime = timers[0];
        _controller.LastOnWallTime = timers[1];
        _controller.LastOnWallRightTime = timers[2];
        _controller.LastOnWallLeftTime = timers[3];
        _controller.LastPressedJumpTime = timers[4];
        _controller.LastPressedDashTime = timers[5];
    }
}
