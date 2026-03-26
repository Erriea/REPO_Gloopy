using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite winBackgroundSprite;
    [SerializeField] private Sprite loseBackgroundSprite;
    [SerializeField] private string winTitle = "You Win";
    [SerializeField] private string loseTitle = "Game Over";
    [SerializeField] private string defaultWinMessage = "You Escaped";
    [SerializeField] private string defaultLoseMessage = "Try Again";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        SetPanelVisible(false);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartScene);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(LoadMainMenu);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartScene);
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(LoadMainMenu);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameManager.GameState gameState, string reason)
    {
        switch (gameState)
        {
            case GameManager.GameState.Win:
                ShowResult(winTitle, string.IsNullOrWhiteSpace(reason) ? defaultWinMessage : reason, winBackgroundSprite);
                break;

            case GameManager.GameState.GameOver:
                ShowResult(loseTitle, string.IsNullOrWhiteSpace(reason) ? defaultLoseMessage : reason, loseBackgroundSprite);
                break;

            default:
                SetPanelVisible(false);
                break;
        }
    }

    private void ShowResult(string title, string message, Sprite backgroundSprite)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (backgroundImage != null)
        {
            backgroundImage.sprite = backgroundSprite;
            backgroundImage.enabled = backgroundSprite != null;
        }

        SetPanelVisible(true);
    }

    private void SetPanelVisible(bool visible)
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(visible);
        }
    }

    private void RestartScene()
    {
        GameLaunchContext.ShowTutorialOnNextGameLoad = false;
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    private void LoadMainMenu()
    {
        if (!string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
