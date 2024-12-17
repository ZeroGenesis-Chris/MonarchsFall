using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    public PlayerData Data => _playerData;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Transform frontWallCheckPoint;
    [SerializeField] private Transform backWallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    
    [Space(10)]

	[Header("Ground Layer")]
    [SerializeField] private LayerMask groundLayer;

    // Components
    public Rigidbody2D RB { get; private set; }

    // Sub-controllers
    private CollisionHandler _collisionHandler;
    private MovementHandler _MovementHandler;
    private MovementHandler _movementHandler;
    private GravityHandler _gravityHandler;
    private InputHandler _inputHandler;
    private IInputActions _inputActions;
    private TimerHandler _timerHandler;

    // Property to access GravityHandler
    public GravityHandler GravityHandler => _gravityHandler;
    public MovementHandler CoreMovement => _MovementHandler;

    // State
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; set; }
    public bool IsWallJumping { get; set; }
    public bool IsDashing { get; set; }
    public bool IsSliding { get; set; }
    public bool IsJumpCut { get; set; }
    public bool IsJumpFalling { get; set; }

    // Timers
    public float LastOnGroundTime { get; set; }
    public float LastOnWallTime { get; set; }
    public float LastOnWallRightTime { get; set; }
    public float LastOnWallLeftTime { get; set; }
    public float LastPressedJumpTime { get; set; }
    public float LastPressedDashTime { get; set; }

    // Input
    public Vector2 MoveInput => _inputHandler.MoveInput;


    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();

        _collisionHandler = new CollisionHandler(this, groundCheckPoint, groundCheckSize, frontWallCheckPoint, backWallCheckPoint, wallCheckSize, groundLayer);
        _MovementHandler = new MovementHandler(this);
        _gravityHandler = new GravityHandler(this, _MovementHandler);
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _inputActions = new UnityInputHandler(playerInput);
        _inputHandler = new InputHandler(this, _inputActions);
        _timerHandler = new TimerHandler(this);
    }

    
    private void OnEnable() {
        //Enable input actions when the PlayerController is enabled
        (_inputActions as UnityInputHandler)?.Enable();
    }

    private void OnDisable() 
    {
        //Disable input actions when the PlayerController is disabled
        (_inputActions as UnityInputHandler)?.Disable();
    }

    private void Start()
    {
        _gravityHandler.SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
    }

    
    

    private void Update()
    {
        _timerHandler.UpdateTimers();
        _inputHandler.HandleInput();
        _collisionHandler.CheckCollisions();
        _MovementHandler.UpdateMovement();
        _gravityHandler.UpdateGravity();
    }

    private void FixedUpdate()
    {
        _MovementHandler.FixedUpdateMovement();
    }


    public void SetFacingDirection(bool isFacingRight)
    {
        if (IsFacingRight != isFacingRight)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            IsFacingRight = isFacingRight;
        }
    }

    public void RequestRefillDash(int amount)
    {
        _MovementHandler.DashRefilling = true;
        StartCoroutine(RefillDash(amount));
        _MovementHandler.DashRefilling = false;
        _MovementHandler.DashesLeft = Mathf.Min(_playerData.dashAmount, _MovementHandler.DashesLeft + 1);
    }

    private IEnumerator RefillDash(int amount)
    {
        yield return new WaitForSeconds(_playerData.dashRefillTime);
    }

    // Movement methods
    public void Jump() => _MovementHandler.Jump();
    public void WallJump(int direction) => _MovementHandler.WallJump(direction);
    public void StartDash(Vector2 direction) => _MovementHandler.StartDash(direction);

    // Check methods
    public bool CanJump() => _MovementHandler.CanJump();
    public bool CanWallJump() => _MovementHandler.CanWallJump();
    public bool CanJumpCut() => _MovementHandler.CanJumpCut();
    public bool CanWallJumpCut() => _MovementHandler.CanWallJumpCut();
    public bool CanDash() => _MovementHandler.CanDash();
    public bool CanSlide() => _MovementHandler.CanSlide();

    // Utility methods
    public void Sleep(float duration) => StartCoroutine(_MovementHandler.PerformSleep(duration));
}