using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action<int> OnLivesChanged;
    public static event Action<int> OnResourcesChanged;

    private int _lives = 5;
    private int _resources = 175;
    public int Resources => _resources;

    private float _gameSpeed = 1f;
    public float GameSpeed => _gameSpeed;

    [SerializeField] private LayerMask platformLayerMask;
    private GameObject selectedTower;
    private Platform originalPlatform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();
                if (platform != null)
                {
                    //Move tower if there is a tower selected and the next clicked platform is available
                    if((selectedTower != null) && !platform.IsOccupied())
                    {
                        //change tower animation
                        //Free old platform
                        originalPlatform.freePlatform(); //must keep reference to original platform
                        //Reserve new platform
                        platform.SetTower(selectedTower);
                        selectedTower.transform.SetParent(platform.transform); //must make the tower the hierarchy child of the new platform
                        //move the tower prefab object
                        Tower selectedTowerScript = selectedTower.GetComponent<Tower>();
                        selectedTowerScript.ToggleTowerHighlight();
                        selectedTowerScript.moveTo(platform.transform.position + new Vector3(0f, 0.5f, 0f));
                        //reset selection
                        selectedTower = null;

                    }
                    //if there is a new tower being selected, unselect the old one
                    else if(selectedTower != null)
                    {
                        Tower selectedTowerScript = selectedTower.GetComponent<Tower>();
                        selectedTowerScript.ToggleTowerHighlight();
                        selectedTower = platform.GetTower();
                        selectedTowerScript.ToggleTowerHighlight();
                    }
                    //select new tower
                    else if(selectedTower == null && platform.IsOccupied())
                    {
                        selectedTower = platform.GetTower();
                        originalPlatform = platform;
                        Tower selectedTowerScript = selectedTower.GetComponent<Tower>();
                        selectedTowerScript.ToggleTowerHighlight();
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        OnLivesChanged?.Invoke(_lives);
        OnResourcesChanged?.Invoke(_resources);
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _lives = Mathf.Max(0, _lives - data.damage);
        OnLivesChanged?.Invoke(_lives);
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        AddResources(Mathf.RoundToInt(enemy.Data.resourceReward));
    }

    private void AddResources(int amount)
    {
        _resources += amount;
        OnResourcesChanged?.Invoke(_resources);
    }

    // for pausing/unpausing, UI needs
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    // for game speed buttons
    public void SetGameSpeed(float newSpeed)
    {
        _gameSpeed = newSpeed;
        SetTimeScale(_gameSpeed);
    }

    public void SpendResources(int amount)
    {
        if (_resources >= amount)
        {
            _resources -= amount;
            OnResourcesChanged?.Invoke(_resources);
        }
    }

    public void ResetGameState()
    {
        _lives = LevelManager.Instance.CurrentLevel.startingLives;
        OnLivesChanged?.Invoke(_lives);
        _resources = LevelManager.Instance.CurrentLevel.startingResources;
        OnResourcesChanged?.Invoke(_resources);

        SetGameSpeed(1f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null)
        {
            ResetGameState();
        }
    }
}
