using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    public float gravityRising = 2f;
    public float gravityFalling = 3.5f;

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

    Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnRestart += ResetPlayer;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnRestart -= ResetPlayer;
    }

    void ResetPlayer()
    {
        isDead = false;
        isGrounded = false;
        frameIndex = 0;
        frameTimer = 0f;
        rb.gravityScale = 1f;
        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;
        if (runFrames != null && runFrames.Length > 0)
            sr.sprite = runFrames[0];
    }

    void Update()
    {
        if (isDead || GameManager.Instance == null) return;
        if (!GameManager.Instance.IsPlaying) return;

        bool mouseClick = Mouse.current != null
                       && Mouse.current.leftButton.wasPressedThisFrame
                       && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject());

        bool jumpPressed = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                        || mouseClick;
        if (isGrounded && jumpPressed)
        {
            rb.linearVelocity = new Vector2(0f, jumpForce);
            isGrounded = false;
        }

        rb.gravityScale = rb.linearVelocity.y < 0f ? gravityFalling : gravityRising;

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
        if (col.gameObject.GetComponent<GroundMarker>() != null)
            isGrounded = true;

        if (col.gameObject.GetComponent<ObstacleMarker>() != null)
            Die();
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<GroundMarker>() != null)
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<GroundMarker>() != null)
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
