using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Tutorial,
        Playing,
        GameOver,
        Win
    }

    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState currentState = GameState.Playing;

    public GameState CurrentState => currentState;
    public string LastResultReason { get; private set; }
    public float ElapsedRoundTime { get; private set; }
    public event System.Action<GameState, string> GameStateChanged;

    private bool roundTimerActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (currentState == GameState.Playing)
        {
            GameStateChanged?.Invoke(currentState, LastResultReason);
        }
    }

    private void Update()
    {
        if (roundTimerActive)
        {
            ElapsedRoundTime += Time.deltaTime;
        }
    }

    public void SetState(GameState newState, string reason = "")
    {
        currentState = newState;
        LastResultReason = reason;
        GameStateChanged?.Invoke(currentState, LastResultReason);
    }

    public bool IsGameplayLocked()
    {
        return currentState != GameState.Playing;
    }

    public void TriggerGameOver(string reason)
    {
        roundTimerActive = false;
        SetState(GameState.GameOver, reason);
    }

    public void TriggerWin(string reason = "You Escaped")
    {
        roundTimerActive = false;
        SetState(GameState.Win, reason);
    }

    public void BeginTutorial()
    {
        roundTimerActive = false;
        SetState(GameState.Tutorial);
    }

    public void EndTutorial()
    {
        ElapsedRoundTime = 0f;
        roundTimerActive = true;
        SetState(GameState.Playing);
    }

    public string GetFormattedElapsedTime()
    {
        return FormatTime(ElapsedRoundTime);
    }

    public static string FormatTime(float timeInSeconds)
    {
        int totalSeconds = Mathf.FloorToInt(timeInSeconds);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }
}
