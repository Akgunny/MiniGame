using UnityEngine;

// ============================================================
//  OBSTACLE SPRITES — read this before adding pixel art
// ============================================================
//  Sprites are assigned by ObstacleSpawner at spawn time.
//  Add your obstacle images to:  Assets/Art/Obstacles/
//
//  Suggested theming (Chrome dino cacti equivalents):
//    Assets/Art/Obstacles/bug.png          ← small obstacle
//    Assets/Art/Obstacles/bug_tall.png     ← tall obstacle
//    Assets/Art/Obstacles/firewall.png     ← wide obstacle
//
//  Then drag them into the "Obstacle Sprites" array on the
//  ObstacleSpawner GameObject in the scene.
// ============================================================

public class Obstacle : MonoBehaviour
{
    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying) return;

        transform.position += Vector3.left * GameManager.Instance.GameSpeed * Time.deltaTime;

        if (transform.position.x < -20f)
            Destroy(gameObject);
    }
}
