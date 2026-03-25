using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        GameOver,
        Win
    }

    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState currentState = GameState.Playing;

    public GameState CurrentState => currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetState(GameState newState)
    {
        currentState = newState;
    }

    public bool IsGameplayLocked()
    {
        return currentState != GameState.Playing;
    }
}
