using UnityEngine;

// ============================================================
//  GROUND SPRITE — drop pixel art here
// ============================================================
//  The ground is two tiles that leapfrog to create an infinite
//  scrolling effect (same trick as the Chrome dino game).
//
//  To use a sprite instead of the grey placeholder:
//    1. Import your ground tile to:  Assets/Art/Ground/ground_tile.png
//    2. Set Wrap Mode to "Repeat" in the texture import settings
//    3. Assign it to the groundSprite field in the Inspector
//
//  The tile should be a horizontal strip (e.g. 512x64 px).
//  It will be stretched to fill the tile width automatically.
// ============================================================

public class ScrollingGround : MonoBehaviour
{
    [Header("Ground Sprite — drop pixel art here")]
    public Sprite groundSprite;  // Assets/Art/Ground/ground_tile.png

    private Transform tileA;
    private Transform tileB;
    private float tileWidth;

    public void Init(Transform a, Transform b, float width)
    {
        tileA = a;
        tileB = b;
        tileWidth = width;

        if (groundSprite != null)
        {
            ApplySprite(tileA);
            ApplySprite(tileB);
        }
    }

    void ApplySprite(Transform tile)
    {
        var sr = tile.gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = groundSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(tileWidth, tile.localScale.y);
        tile.GetComponent<Renderer>().enabled = false; // hide mesh renderer
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying) return;

        float speed = GameManager.Instance.GameSpeed;
        tileA.position += Vector3.left * speed * Time.deltaTime;
        tileB.position += Vector3.left * speed * Time.deltaTime;

        if (tileA.position.x + tileWidth * 0.5f <= -tileWidth * 0.5f)
            tileA.position = new Vector3(tileB.position.x + tileWidth, tileA.position.y, 0f);

        if (tileB.position.x + tileWidth * 0.5f <= -tileWidth * 0.5f)
            tileB.position = new Vector3(tileA.position.x + tileWidth, tileB.position.y, 0f);
    }
}
