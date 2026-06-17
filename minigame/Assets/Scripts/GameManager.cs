using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float GameSpeed { get; private set; } = 8f;
    public bool IsPlaying { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }

    public event System.Action OnRestart;

    private const float StartSpeed = 8f;
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
            if (JumpPressed())
                Restart();
            return;
        }

        if (!IsPlaying)
        {
            if (JumpPressed())
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

    static bool JumpPressed()
    {
        bool mouseClick = Mouse.current != null
                       && Mouse.current.leftButton.wasPressedThisFrame
                       && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject());

        return (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            || mouseClick;
    }

    public void TriggerGameOver()
    {
        IsPlaying = false;
        IsGameOver = true;
    }

    void Restart()
    {
        GameSpeed = StartSpeed;
        Score = 0;
        scoreTimer = 0f;
        IsGameOver = false;
        IsPlaying = false;
        OnRestart?.Invoke();
    }
}
