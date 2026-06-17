using UnityEngine;
using UnityEngine.UI;

// ============================================================
//  GameBootstrapper
//  Attach this to ANY empty GameObject in your scene and press
//  Play — it builds the entire game world at runtime.
//  No manual scene setup required.
// ============================================================

public class GameBootstrapper : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────
    //  SPRITE SLOTS — assign these in the Inspector
    //  after importing your pixel art. Everything below is
    //  optional; the game runs fine with placeholder shapes.
    // ──────────────────────────────────────────────────────────

    [Header("Player Sprites  →  Assets/Art/Player/")]
    public Sprite playerRunFrame0;   // claude_run_0.png  ┐ run cycle
    public Sprite playerRunFrame1;   // claude_run_1.png  ┘ (add more frames to PlayerController.runFrames)
    public Sprite playerJumpSprite;  // claude_jump.png
    public Sprite playerDeadSprite;  // claude_dead.png

    [Header("Obstacle Sprites  →  Assets/Art/Obstacles/")]
    public Sprite[] obstacleSprites; // bug.png, firewall.png, etc.
    public float[]  obstacleHeights; // world-unit height matching each sprite above

    [Header("Ground Sprite  →  Assets/Art/Ground/")]
    public Sprite groundSprite;      // ground_tile.png  (set Wrap Mode: Repeat)

    [Header("Font  →  Assets/Art/Fonts/")]
    public Font scoreFont;           // PressStart2P.ttf (free on Google Fonts)

    // ──────────────────────────────────────────────────────────
    //  Layout constants — tweak if things look off
    // ──────────────────────────────────────────────────────────
    const float GroundY         = -3.7f;   // world Y of the ground top surface
    const float GroundThickness = 0.5f;
    const float TileWidth       = 24f;
    const float PlayerStartX    = -6f;
    const float PlayerSize      = 1f;      // world units (placeholder square side)

    void Awake()
    {
        SetupCamera();
        SetupTags();

        var gm = SetupGameManager();
        SetupGround();
        var player = SetupPlayer();
        SetupObstacleSpawner();
        SetupUI();
    }

    // ── Camera ───────────────────────────────────────────────

    void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.backgroundColor = new Color(0.95f, 0.95f, 0.95f); // light grey like Chrome
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    // ── Tags (created at runtime if missing) ─────────────────

    void SetupTags()
    {
        // Tags must exist in Project Settings > Tags & Layers.
        // If you see "Tag: Ground not found" warnings, add "Ground"
        // and "Obstacle" in Edit > Project Settings > Tags & Layers.
    }

    // ── GameManager ──────────────────────────────────────────

    GameManager SetupGameManager()
    {
        var go = new GameObject("GameManager");
        return go.AddComponent<GameManager>();
    }

    // ── Ground ───────────────────────────────────────────────

    void SetupGround()
    {
        var root = new GameObject("Ground");
        var scroller = root.AddComponent<ScrollingGround>();
        scroller.groundSprite = groundSprite;

        var tileA = MakeGroundTile("TileA", 0f);
        var tileB = MakeGroundTile("TileB", TileWidth);
        tileA.SetParent(root.transform);
        tileB.SetParent(root.transform);

        scroller.Init(tileA, tileB, TileWidth);
    }

    Transform MakeGroundTile(string name, float offsetX)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.tag = "Ground";
        go.transform.position = new Vector3(offsetX, GroundY - GroundThickness * 0.5f, 0f);
        go.transform.localScale = new Vector3(TileWidth, GroundThickness, 1f);

        // grey placeholder colour
        go.GetComponent<Renderer>().material.color = new Color(0.55f, 0.55f, 0.55f);

        // 2D collider
        Object.Destroy(go.GetComponent<BoxCollider>());
        go.AddComponent<BoxCollider2D>();

        return go.transform;
    }

    // ── Player ───────────────────────────────────────────────

    GameObject SetupPlayer()
    {
        var go = new GameObject("ClaudePlayer");
        go.tag = "Player";
        go.transform.position = new Vector3(PlayerStartX, GroundY + PlayerSize * 0.5f, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.18f, 0.18f, 0.18f); // dark placeholder (Claude's terminal black)

        // Assign sprites if provided
        var pc = go.AddComponent<PlayerController>();
        pc.jumpSprite = playerJumpSprite;
        pc.deadSprite = playerDeadSprite;

        if (playerRunFrame0 != null || playerRunFrame1 != null)
        {
            // only include non-null frames
            int count = (playerRunFrame0 != null ? 1 : 0) + (playerRunFrame1 != null ? 1 : 0);
            pc.runFrames = new Sprite[count];
            int i = 0;
            if (playerRunFrame0 != null) pc.runFrames[i++] = playerRunFrame0;
            if (playerRunFrame1 != null) pc.runFrames[i++] = playerRunFrame1;
        }

        var rb = go.AddComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 1f); // slightly inset so edge clips feel fair

        // Placeholder square if no sprites assigned
        if (playerRunFrame0 == null && playerJumpSprite == null)
            go.transform.localScale = new Vector3(PlayerSize, PlayerSize, 1f);

        return go;
    }

    // ── Obstacle Spawner ─────────────────────────────────────

    void SetupObstacleSpawner()
    {
        var go = new GameObject("ObstacleSpawner");
        var spawner = go.AddComponent<ObstacleSpawner>();
        spawner.obstacleSprites = obstacleSprites;
        spawner.obstacleHeights = obstacleHeights;
    }

    // ── UI Canvas ────────────────────────────────────────────

    void SetupUI()
    {
        var canvasGo = new GameObject("UICanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        var ui = canvasGo.AddComponent<ScoreUI>();
        ui.scoreFont = scoreFont;
        ui.Init(canvas);
    }
}
