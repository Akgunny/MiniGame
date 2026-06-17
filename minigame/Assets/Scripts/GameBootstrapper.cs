using UnityEngine;
using UnityEngine.UI;

// ============================================================
//  GameBootstrapper
//  Creates itself automatically at runtime — just press Play.
//  No scene setup needed. To assign sprites/font, place this
//  script on a GameObject and use the Inspector fields.
// ============================================================

public class GameBootstrapper : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindObjectOfType<GameBootstrapper>() != null) return;
        var go = new GameObject("GameBootstrapper");
        go.AddComponent<GameBootstrapper>();
    }

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
        SetupGameManager();
        SetupGround();
        SetupPlayer();
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

    // ── GameManager ──────────────────────────────────────────

    GameManager SetupGameManager()
    {
        return new GameObject("GameManager").AddComponent<GameManager>();
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
        go.AddComponent<GroundMarker>();
        go.transform.position = new Vector3(offsetX, GroundY - GroundThickness * 0.5f, 0f);
        go.transform.localScale = new Vector3(TileWidth, GroundThickness, 1f);

        // grey placeholder colour
        go.GetComponent<Renderer>().material.color = new Color(0.55f, 0.55f, 0.55f);

        // 2D collider — DestroyImmediate so the 3D one is gone before we add the 2D one
        DestroyImmediate(go.GetComponent<BoxCollider>());
        go.AddComponent<BoxCollider2D>();

        return go.transform;
    }

    // ── Player ───────────────────────────────────────────────

    GameObject SetupPlayer()
    {
        var go = new GameObject("ClaudePlayer");
        go.transform.position = new Vector3(PlayerStartX, GroundY + PlayerSize * 0.5f, 0f);

        // AddComponent<PlayerController> triggers RequireComponent, auto-adding Rigidbody2D,
        // BoxCollider2D, and SpriteRenderer — so we configure them afterwards.
        var pc = go.AddComponent<PlayerController>();
        var sr = go.GetComponent<SpriteRenderer>();
        pc.jumpSprite = playerJumpSprite != null ? playerJumpSprite : MakePlaceholderSprite(new Color(0.18f, 0.18f, 0.18f));
        pc.deadSprite = playerDeadSprite != null ? playerDeadSprite : MakePlaceholderSprite(new Color(0.6f, 0.1f, 0.1f));

        if (playerRunFrame0 != null || playerRunFrame1 != null)
        {
            int count = (playerRunFrame0 != null ? 1 : 0) + (playerRunFrame1 != null ? 1 : 0);
            pc.runFrames = new Sprite[count];
            int i = 0;
            if (playerRunFrame0 != null) pc.runFrames[i++] = playerRunFrame0;
            if (playerRunFrame1 != null) pc.runFrames[i++] = playerRunFrame1;
        }
        else
        {
            // placeholder run "animation" — just one dark square
            pc.runFrames = new Sprite[] { MakePlaceholderSprite(new Color(0.18f, 0.18f, 0.18f)) };
        }

        // seed the renderer with a sprite so it's visible from frame 0
        sr.sprite = pc.runFrames[0];

        var rb = go.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 2f;

        var col = go.GetComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 1f);
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

    // ── Placeholder sprite generator ─────────────────────────

    public static Sprite MakePlaceholderSprite(Color color)
    {
        var tex = new Texture2D(4, 4);
        var pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
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
