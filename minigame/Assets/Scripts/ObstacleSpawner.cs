using UnityEngine;

// ============================================================
//  OBSTACLE SPRITES — drop pixel art here
// ============================================================
//  Add your obstacle sprites to:  Assets/Art/Obstacles/
//  Then drag them into the obstacleSprites array in the
//  Inspector on this component.
//
//  Each entry in obstacleSprites can be a different type of
//  obstacle (small bug, tall bug, firewall, etc).
//  One is chosen at random each spawn.
//
//  obstacleHeights should match the pixel art height in world
//  units — tweak until the obstacle sits on the ground.
// ============================================================

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn timing")]
    public float minInterval = 1.2f;
    public float maxInterval = 2.8f;

    [Header("Obstacle Sprites — drop pixel art here")]
    public Sprite[] obstacleSprites;

    [Header("Per-sprite world height (match your art)")]
    public float[] obstacleHeights;   // index matches obstacleSprites

    private const float SpawnX = 13f;
    private const float GroundSurfaceY = -3.5f; // must match ground top edge in bootstrapper
    private const float PlaceholderWidth = 0.8f;
    private const float PlaceholderHeight = 1.5f;

    private float timer;
    private float nextSpawn;

    void Start() => nextSpawn = 1.5f;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying) return;

        timer += Time.deltaTime;
        if (timer >= nextSpawn)
        {
            Spawn();
            timer = 0f;
            nextSpawn = Random.Range(minInterval, maxInterval);
        }
    }

    void Spawn()
    {
        int pick = (obstacleSprites != null && obstacleSprites.Length > 0)
            ? Random.Range(0, obstacleSprites.Length)
            : -1;

        float height = (pick >= 0 && obstacleHeights != null && pick < obstacleHeights.Length)
            ? obstacleHeights[pick]
            : PlaceholderHeight;

        float width = PlaceholderWidth;

        var go = new GameObject("Obstacle");
        go.tag = "Obstacle";
        go.transform.position = new Vector3(SpawnX, GroundSurfaceY + height * 0.5f, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        if (pick >= 0)
        {
            sr.sprite = obstacleSprites[pick];
            go.transform.localScale = Vector3.one;
        }
        else
        {
            // red placeholder rectangle
            sr.color = new Color(0.85f, 0.2f, 0.2f);
            go.transform.localScale = new Vector3(width, height, 1f);
        }

        var col = go.AddComponent<BoxCollider2D>();
        if (pick >= 0)
            col.size = new Vector2(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);

        go.AddComponent<Obstacle>();
    }
}
