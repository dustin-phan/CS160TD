using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip towerPlaceSound;
    [SerializeField] private AudioClip towerDestroySound;
    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField] private AudioClip enemyBreachSound;
    [SerializeField] private AudioClip waveStartSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip buttonClickSound;

    private AudioSource _audioSource;
    private bool _gameOverPlayed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("SoundManager");
        go.AddComponent<SoundManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();

        if (towerPlaceSound == null)   towerPlaceSound   = Resources.Load<AudioClip>("Audio/tower_place");
        if (towerDestroySound == null) towerDestroySound = Resources.Load<AudioClip>("Audio/tower_destroy");
        if (enemyDeathSound == null)   enemyDeathSound   = Resources.Load<AudioClip>("Audio/enemy_death");
        if (enemyBreachSound == null)  enemyBreachSound  = Resources.Load<AudioClip>("Audio/enemy_breach");
        if (waveStartSound == null)    waveStartSound    = Resources.Load<AudioClip>("Audio/wave_start");
        if (gameOverSound == null)     gameOverSound     = Resources.Load<AudioClip>("Audio/game_over");
        if (victorySound == null)      victorySound      = Resources.Load<AudioClip>("Audio/victory");
        if (buttonClickSound == null)  buttonClickSound  = Resources.Load<AudioClip>("Audio/button_click");
    }

    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Spawner.OnWaveChanged += HandleWaveChanged;
        Spawner.OnMissionComplete += HandleMissionComplete;
        GameManager.OnLivesChanged += HandleLivesChanged;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Spawner.OnWaveChanged -= HandleWaveChanged;
        Spawner.OnMissionComplete -= HandleMissionComplete;
        GameManager.OnLivesChanged -= HandleLivesChanged;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleEnemyDestroyed(Enemy enemy) => PlaySFX(enemyDeathSound);
    private void HandleEnemyReachedEnd(EnemyData data) => PlaySFX(enemyBreachSound);
    private void HandleWaveChanged(int wave) => PlaySFX(waveStartSound);
    private void HandleMissionComplete() => PlaySFX(victorySound);

    private void HandleLivesChanged(int lives)
    {
        if (lives <= 0 && !_gameOverPlayed)
        {
            PlaySFX(gameOverSound);
            _gameOverPlayed = true;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _gameOverPlayed = false;
        foreach (Button btn in FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            if (btn.GetComponent<SoundButton>() == null)
                btn.gameObject.AddComponent<SoundButton>();
        }
    }

    public AudioClip GetShootSound(string towerName)
    {
        if (towerName.Contains("Archer")) return Resources.Load<AudioClip>("Audio/archer_shoot");
        if (towerName.Contains("Knight")) return Resources.Load<AudioClip>("Audio/knight_shoot");
        if (towerName.Contains("Wizard")) return Resources.Load<AudioClip>("Audio/wizard_shoot");
        return null;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            _audioSource.PlayOneShot(clip);
    }

    public void PlayTowerPlace() => PlaySFX(towerPlaceSound);
    public void PlayTowerDestroy() => PlaySFX(towerDestroySound);
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
}
