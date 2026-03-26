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
    public event System.Action<GameState, string> GameStateChanged;

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
        SetState(GameState.GameOver, reason);
    }

    public void TriggerWin(string reason = "You Escaped")
    {
        SetState(GameState.Win, reason);
    }

    public void BeginTutorial()
    {
        SetState(GameState.Tutorial);
    }

    public void EndTutorial()
    {
        SetState(GameState.Playing);
    }
}
