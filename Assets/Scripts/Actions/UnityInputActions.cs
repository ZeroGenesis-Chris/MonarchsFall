using UnityEngine;
using UnityEngine.InputSystem;

/**
 * This class handles the Input Actions for the player.
 */
public class UnityInputActions : IInputActions
{
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;

    /**
     * The current move input
     */
    public Vector2 MoveInput => _moveAction.ReadValue<Vector2>();

    /**
     * Whether the jump button was pressed this frame
     */
    public bool JumpPressed => _jumpAction.WasPressedThisFrame();

    /**
     * Whether the jump button was released this frame
     */
    public bool JumpReleased => _jumpAction.WasReleasedThisFrame();

    /**
     * Whether the jump button is currently being held down
     */
    public bool JumpHeld => _jumpAction.IsPressed();

    /**
     * Whether the dash button was pressed this frame
     */
    public bool DashPressed => _dashAction.WasPressedThisFrame();

    /**
     * Constructor
     * 
     * @param playerInput the PlayerInput component
     */
    public UnityInputActions(PlayerInput playerInput)
    {
        // Store the PlayerInput component
        _playerInput = playerInput;

        // Get the actions from the PlayerInput component
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _dashAction = _playerInput.actions["Dash"];
    }

    /**
     * Enable the input actions
     */
    public void Enable()
    {
        _moveAction.Enable();
        _jumpAction.Enable();
        _dashAction.Enable();
    }

    /**
     * Disable the input actions
     */
    public void Disable()
    {
        _moveAction.Disable();
        _jumpAction.Disable();
        _dashAction.Disable();
    }
}


