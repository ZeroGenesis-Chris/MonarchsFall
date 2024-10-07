using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    public PlayerData Data => _playerData;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Transform frontWallCheckPoint;
    [SerializeField] private Transform backWallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    [SerializeField] private LayerMask groundLayer;

    // Components
    public Rigidbody2D RB { get; private set; }

    // Sub-controllers
    private CollisionHandler _collisionHandler;
    private CoreMovement _coreMovement;
    private GravityHandler _gravityHandler;
    private InputHandler _inputHandler;
    private TimerHandler _timerHandler;

    // Property to access GravityHandler
    public GravityHandler GravityHandler => _gravityHandler;
    public CoreMovement CoreMovement => _coreMovement;

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
        _coreMovement = new CoreMovement(this);
        _gravityHandler = new GravityHandler(this, _coreMovement);
        _inputHandler = new InputHandler(this);
        _timerHandler = new TimerHandler(this);
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
        _coreMovement.UpdateMovement();
        _gravityHandler.UpdateGravity();
    }

    private void FixedUpdate()
    {
        _coreMovement.FixedUpdateMovement();
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
        _coreMovement.DashRefilling = true;
        StartCoroutine(RefillDash(amount));
        _coreMovement.DashRefilling = false;
        _coreMovement.DashesLeft = Mathf.Min(_playerData.dashAmount, _coreMovement.DashesLeft + 1);
    }

    private IEnumerator RefillDash(int amount)
    {
        yield return new WaitForSeconds(_playerData.dashRefillTime);
    }

    // Movement methods
    public void Jump() => _coreMovement.Jump();
    public void WallJump(int direction) => _coreMovement.WallJump(direction);
    public void StartDash(Vector2 direction) => _coreMovement.StartDash(direction);

    // Check methods
    public bool CanJump() => _coreMovement.CanJump();
    public bool CanWallJump() => _coreMovement.CanWallJump();
    public bool CanJumpCut() => _coreMovement.CanJumpCut();
    public bool CanWallJumpCut() => _coreMovement.CanWallJumpCut();
    public bool CanDash() => _coreMovement.CanDash();
    public bool CanSlide() => _coreMovement.CanSlide();

    // Utility methods
    public void Sleep(float duration) => StartCoroutine(_coreMovement.PerformSleep(duration));
}