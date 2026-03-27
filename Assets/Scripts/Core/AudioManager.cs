using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float musicVolume = 0.6f;

    [Header("UI")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private float buttonClickVolume = 1f;

    [Header("Gameplay")]
    [SerializeField] private AudioClip hostDeathClip;
    [SerializeField] private float hostDeathVolume = 1f;
    [SerializeField] private AudioClip targetHostClip;
    [SerializeField] private float targetHostVolume = 1f;
    [SerializeField] private AudioClip swapHostClip;
    [SerializeField] private float swapHostVolume = 1f;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private float winVolume = 1f;
    [SerializeField] private AudioClip loseClip;
    [SerializeField] private float loseVolume = 1f;

    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private GameManager subscribedGameManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        RefreshGameManagerSubscription();
        UpdateMusicForCurrentContext(restartGameplayMusic: false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (subscribedGameManager != null)
        {
            subscribedGameManager.GameStateChanged -= HandleGameStateChanged;
        }
    }

    public void PlayButtonClick()
    {
        PlayOneShot(buttonClickClip, buttonClickVolume);
    }

    public void PlayHostDeath()
    {
        PlayOneShot(hostDeathClip, hostDeathVolume);
    }

    public void PlayTargetHost()
    {
        PlayOneShot(targetHostClip, targetHostVolume);
    }

    public void PlaySwapHost()
    {
        PlayOneShot(swapHostClip, swapHostVolume);
    }

    public void PlayWin()
    {
        PlayOneShot(winClip, winVolume);
    }

    public void PlayLose()
    {
        PlayOneShot(loseClip, loseVolume);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshGameManagerSubscription();
        UpdateMusicForCurrentContext(restartGameplayMusic: false);
    }

    private void RefreshGameManagerSubscription()
    {
        if (subscribedGameManager != null)
        {
            subscribedGameManager.GameStateChanged -= HandleGameStateChanged;
            subscribedGameManager = null;
        }

        if (GameManager.Instance != null)
        {
            subscribedGameManager = GameManager.Instance;
            subscribedGameManager.GameStateChanged += HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameManager.GameState gameState, string reason)
    {
        switch (gameState)
        {
            case GameManager.GameState.Tutorial:
                StopMusic();
                break;

            case GameManager.GameState.Playing:
                UpdateMusicForCurrentContext(restartGameplayMusic: true);
                break;

            case GameManager.GameState.Win:
                StopMusic();
                break;

            case GameManager.GameState.GameOver:
                StopMusic();
                break;
        }
    }

    private void UpdateMusicForCurrentContext(bool restartGameplayMusic)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        bool isMainMenu = activeScene.name == mainMenuSceneName;

        if (isMainMenu)
        {
            PlayMusic(restartFromBeginning: true);
            return;
        }

        if (GameManager.Instance == null)
        {
            StopMusic();
            return;
        }

        if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            PlayMusic(restartFromBeginning: restartGameplayMusic);
            return;
        }

        StopMusic();
    }

    private void PlayMusic(bool restartFromBeginning)
    {
        if (backgroundMusic == null)
        {
            return;
        }

        musicSource.volume = musicVolume;

        if (musicSource.clip != backgroundMusic)
        {
            musicSource.clip = backgroundMusic;
            restartFromBeginning = true;
        }

        if (restartFromBeginning)
        {
            musicSource.Stop();
            musicSource.time = 0f;
        }

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    private void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, volume);
    }
}
