using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float GameSpeed { get; private set; } = 8f;
    public bool IsPlaying { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }

    private const float SpeedIncreaseRate = 0.4f;
    private const float MaxSpeed = 30f;
    private float scoreTimer;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (!IsPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                IsPlaying = true;
            return;
        }

        GameSpeed = Mathf.Min(GameSpeed + SpeedIncreaseRate * Time.deltaTime, MaxSpeed);

        scoreTimer += Time.deltaTime;
        if (scoreTimer >= 0.1f)
        {
            Score++;
            scoreTimer = 0f;
        }
    }

    public void TriggerGameOver()
    {
        IsPlaying = false;
        IsGameOver = true;
    }
}
