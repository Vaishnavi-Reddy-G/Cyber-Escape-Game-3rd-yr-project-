using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float climbSpeed = 8f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX;
    [SerializeField] private float wallJumpY;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    [Header("Trampoline Settings")] // TRAMPOLINE: New section
    [SerializeField] private float bounceForce = 20f; // how strong the trampoline bounce is
    [SerializeField] private string trampolineTag = "Trampoline"; // tag used for trampoline

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private float verticalInput;
    private bool isClimbing = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Flip player
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        // Climbing
        if (isClimbing)
        {
            body.velocity = new Vector2(0, verticalInput * climbSpeed);
            body.gravityScale = 0;
            anim.SetBool("climb", verticalInput != 0);
            return;
        }
        else
        {
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        // Wall check
        if (onWall())
        {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        }

        if (isGrounded())
        {
            coyoteCounter = coyoteTime;
            jumpCounter = extraJumps;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0) return;

        SoundManager.instance.PlaySound(jumpSound);

        if (onWall())
        {
            WallJump();
        }
        else
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            if (!isGrounded() && coyoteCounter <= 0)
                jumpCounter--;

            coyoteCounter = 0;
        }

        isClimbing = false;
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        wallJumpCooldown = 0;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }

    // Ladder logic
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = true;
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        }

        // TRAMPOLINE: Bounce effect
        if (other.CompareTag(trampolineTag))
        {
            // Reset vertical velocity for consistent bounce
            Vector2 currentVelocity = body.velocity;
            currentVelocity.y = 0;
            body.velocity = currentVelocity;

            // Apply bounce force
            body.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

            // Optional: play bounce animation or sound
            Debug.Log("Bounced on trampoline!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            body.gravityScale = 7;
        }
    }
}
