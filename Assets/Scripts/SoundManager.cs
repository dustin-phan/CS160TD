using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Tower Sounds")]
    [SerializeField] private AudioClip towerPlaceSound;
    [SerializeField] private AudioClip towerDestroySound;

    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField] private AudioClip enemyBreachSound;

    [Header("Wave Sounds")]
    [SerializeField] private AudioClip waveStartSound;

    [Header("Game State Sounds")]
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip victorySound;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonClickSound;

    private AudioSource _audioSource;
    private bool _gameOverPlayed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
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

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) => _gameOverPlayed = false;

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            _audioSource.PlayOneShot(clip);
    }

    public void PlayTowerPlace() => PlaySFX(towerPlaceSound);
    public void PlayTowerDestroy() => PlaySFX(towerDestroySound);
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
}
