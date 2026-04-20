using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LuckSystem))]
[RequireComponent(typeof(PlayerAbilities))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 7f;
    public float jumpForce = 14f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask;

    [Header("Advanced Jump")]
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    private Rigidbody2D rb;
    private LuckSystem luck;
    private PlayerAbilities abilities;

    private float moveInput;
    private bool jumpPressed;
    private bool dashPressed;

    private bool isGrounded;
    private bool hasUsedDoubleJump;
    private float coyoteCounter;
    private float jumpBufferCounter;

    private Vector3 respawnPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        luck = GetComponent<LuckSystem>();
        abilities = GetComponent<PlayerAbilities>();
        respawnPoint = transform.position;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
            jumpBufferCounter = jumpBufferTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            dashPressed = true;

        if (Input.GetKeyDown(KeyCode.R))
            Respawn();

        jumpBufferCounter -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        CheckGround();
        HandleMove();
        HandleJump();
        HandleAbilities();
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            hasUsedDoubleJump = false;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }
    }

    private void HandleMove()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput > 0.01f) transform.localScale = new Vector3(1f, 1f, 1f);
        else if (moveInput < -0.01f) transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    private void HandleJump()
    {
        if (jumpBufferCounter > 0f)
        {
            if (coyoteCounter > 0f)
            {
                DoJump();
                jumpBufferCounter = 0f;
                return;
            }

            if (!isGrounded && !hasUsedDoubleJump && luck.CanDoubleJump)
            {
                if (luck.SpendLuck(10f))
                {
                    DoJump();
                    hasUsedDoubleJump = true;
                    jumpBufferCounter = 0f;
                }
            }
        }

        jumpPressed = false;
    }

    private void HandleAbilities()
    {
        if (dashPressed)
        {
            abilities.TryDash(moveInput);
            dashPressed = false;
        }

        bool holdingJump = Input.GetKey(KeyCode.Space);
        abilities.TrySlowFall(holdingJump);
    }

    private void DoJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void SetRespawnPoint(Vector3 point)
    {
        respawnPoint = point;
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
