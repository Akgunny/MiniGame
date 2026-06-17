using UnityEngine;
using UnityEngine.UI;

// ============================================================
//  FONT — drop pixel font here
// ============================================================
//  By default this uses Unity's built-in Arial font.
//  To match the Chrome dino style, use a pixel/monospace font:
//    1. Import font to:  Assets/Art/Fonts/PressStart2P.ttf
//       (Press Start 2P is free on Google Fonts — very fitting)
//    2. Assign it to the scoreFont field in the Inspector
// ============================================================

public class ScoreUI : MonoBehaviour
{
    [Header("Font — drop pixel font here")]
    public Font scoreFont;   // Assets/Art/Fonts/PressStart2P.ttf

    private Text scoreText;
    private Text statusText;

    public void Init(Canvas canvas)
    {
        scoreText  = MakeText(canvas, "ScoreText",  new Vector2(1f, 1f), new Vector2(-20f, -10f), 28);
        statusText = MakeText(canvas, "StatusText", new Vector2(0.5f, 0.5f), Vector2.zero, 22);
        statusText.text = "PRESS SPACE TO START";
    }

    Text MakeText(Canvas canvas, string name, Vector2 anchor, Vector2 anchoredPos, int size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(canvas.transform, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(400f, 60f);

        var t = go.AddComponent<Text>();
        t.font = scoreFont != null ? scoreFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size;
        t.color = new Color(0.3f, 0.3f, 0.3f);
        t.alignment = (anchor.x > 0.6f) ? TextAnchor.UpperRight : TextAnchor.MiddleCenter;
        return t;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null)
            scoreText.text = GameManager.Instance.Score.ToString("D5");

        if (statusText != null)
        {
            if (!GameManager.Instance.IsPlaying && !GameManager.Instance.IsGameOver)
                statusText.text = "PRESS SPACE TO START";
            else if (GameManager.Instance.IsGameOver)
                statusText.text = "GAME OVER\nPRESS SPACE TO RESTART";
            else
                statusText.text = "";
        }
    }
}
