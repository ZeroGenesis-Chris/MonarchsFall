using UnityEngine;

public interface IInputActions
{
    Vector2 MoveInput { get; }
    bool JumpPressed { get; }
    bool JumpReleased { get; }
    bool JumpHeld { get; }
    bool DashPressed { get; }
}
