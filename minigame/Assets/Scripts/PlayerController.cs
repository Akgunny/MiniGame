using UnityEngine;

// ============================================================
//  SPRITE SETUP — read this before adding pixel art
// ============================================================
//  1. Import your sprites into:  Assets/Art/Player/
//
//  2. In the Inspector, assign each field under "Player Sprites":
//
//     Run Sprite  →  Assets/Art/Player/claude_run.png
//                    (or a sprite sheet — set to Sprite Mode: Multiple
//                     and slice the frames; animation is handled below)
//
//     Jump Sprite →  Assets/Art/Player/claude_jump.png
//
//     Dead Sprite →  Assets/Art/Player/claude_dead.png
//
//  3. For run animation, add each frame to runFrames[] in order.
//     The animator cycles through them at runFrameRate fps.
//
//  If no sprites are assigned the character shows as a white square
//  (placeholder) — the game is fully playable without art.
// ============================================================

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 14f;

    [Header("Player Sprites — drop pixel art here")]
    public Sprite jumpSprite;
    public Sprite deadSprite;
    public Sprite[] runFrames;   // frames in order for run animation
    public float runFrameRate = 10f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isGrounded;
    private bool isDead;
    private float frameTimer;
    private int frameIndex;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead || GameManager.Instance == null) return;
        if (!GameManager.Instance.IsPlaying) return;

        bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
        if (isGrounded && jumpPressed)
        {
            rb.linearVelocity = new Vector2(0f, jumpForce);
            isGrounded = false;
        }

        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (!isGrounded)
        {
            if (jumpSprite != null) sr.sprite = jumpSprite;
            return;
        }

        if (runFrames == null || runFrames.Length == 0) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= 1f / runFrameRate)
        {
            frameTimer = 0f;
            frameIndex = (frameIndex + 1) % runFrames.Length;
            sr.sprite = runFrames[frameIndex];
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (col.gameObject.CompareTag("Obstacle"))
            Die();
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        if (deadSprite != null) sr.sprite = deadSprite;
        GameManager.Instance.TriggerGameOver();
    }
}
