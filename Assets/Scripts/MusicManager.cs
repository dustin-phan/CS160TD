using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip backgroundMusic;

    private AudioSource _audioSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject("MusicManager");
        go.AddComponent<MusicManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;

        if (backgroundMusic == null)
            backgroundMusic = Resources.Load<AudioClip>("Audio/music_bg");
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            _audioSource.clip = backgroundMusic;
            _audioSource.Play();
        }
    }
}
