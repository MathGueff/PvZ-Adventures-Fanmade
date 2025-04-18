using UnityEngine;
public enum GameState
{
    Started,
    NotStarted,
    Ended,
}

public enum Character
{
    Plant,
    Zombie
}

public class GameController : MonoBehaviour
{
    #region Singleton
    public static GameController instance;

    private void Awake()
    {
        // Garantir que apenas uma inst�ncia do GameController exista
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroi a nova inst�ncia caso j� exista uma
        }
        else
        {
            instance = this; // Se n�o existir, define a inst�ncia
        }
    }
    #endregion

    [Header("States")]
    public GameState gameState;

    [Header("Level Info")]
    public LevelDataScriptable levelData;
    public static LevelDataScriptable LevelData => instance.levelData;

    [Header("Sounds")]
    [SerializeField] private AudioClip readySetPlantSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip pickAPlantMusic;

    private void Start()
    {
        gameState = GameState.NotStarted;
        MusicController.instance.PlayMusic(pickAPlantMusic);
    }

    public void StartGame()
    {
        if(gameState == GameState.NotStarted)
        {
            if (levelData == null || levelData.ZombieScriptables == null)
            {
                Debug.Log("Level Data ou ZombieScriptablesManager n�o atribu�do");
                return;
            }
            MusicController.instance.StopCurrentMusic();
            SoundManager.instance.PlaySound(readySetPlantSound);
            Invoke("StartMusic", 3f);
        }
    }

    public void StartMusic()
    {
        MusicController.instance.PlayRandomMusic();
        gameState = GameState.Started;
    }

    public void PauseGame()
    {
        if (gameState == GameState.Started)
        {
            gameState = GameState.NotStarted;
        }
    }

    public void OnZombieDied(Zombie z)
    {
        if (ZombiesManager.instance.allZombiesAlive.Count == 0 && 
            ZombiesManager.instance.totalZombiesSpawned == ZombiesManager.instance.totalZombiesInLevel)
        {
            gameState = GameState.Ended;
            SoundManager.instance.StopSound();
            MusicController.instance.SetIsLoop(false);
            MusicController.instance.PlayMusic(victorySound);
        }
    }

    public void OnGameOver()
    {
        Debug.Log("Os zumbis devoraram seu c�rebro, GAME OVER");
        gameState = GameState.Ended;
        SoundManager.instance.StopSound();
        MusicController.instance.SetIsLoop(false);
        MusicController.instance.PlayMusic(gameOverSound);
    }
}
