using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";

    public void PlayGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameLaunchContext.ShowTutorialOnNextGameLoad = true;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
