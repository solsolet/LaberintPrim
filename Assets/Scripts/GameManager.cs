using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton: 1 GameManager, accessible from anywhere
    public static GameManager Instance { get; private set; }

    // ---------- Level configuration ----------
    [System.Serializable]
    public class LevelConfig
    {
        public int mazeWidth         = 11;
        public int mazeHeight        = 11;
        public int holeCount         = 2;
        public int lives             = 3;
        public int smallCollectables = 4;
        public int mediumCollectables= 2;
        public int largeCollectables = 1;
    }

    // Filled in Inspector
    public LevelConfig[] levels = new LevelConfig[]
    {
        new LevelConfig { mazeWidth=11, mazeHeight=11, holeCount=2,  lives=3, smallCollectables=4, mediumCollectables=2, largeCollectables=1 },
        new LevelConfig { mazeWidth=13, mazeHeight=13, holeCount=4,  lives=3, smallCollectables=5, mediumCollectables=3, largeCollectables=1 },
        new LevelConfig { mazeWidth=15, mazeHeight=15, holeCount=6,  lives=2, smallCollectables=6, mediumCollectables=3, largeCollectables=2 },
        new LevelConfig { mazeWidth=17, mazeHeight=17, holeCount=9,  lives=2, smallCollectables=7, mediumCollectables=4, largeCollectables=2 },
        new LevelConfig { mazeWidth=21, mazeHeight=21, holeCount=13, lives=1, smallCollectables=8, mediumCollectables=5, largeCollectables=3 },
    };

    // ---------- Runtime state ----------
    [HideInInspector] public int currentLevel = 0;
    [HideInInspector] public int lives        = 3;
    [HideInInspector] public int score        = 0;

    // ---------- References (assign in Inspector) ----------
    public MazeRenderer mazeRenderer;
    public UIManager    uiManager;

    // ---------- Audio ----------
    [Header("Audio clips (drag from Assets/Audio/)")]
    public AudioClip collectSound;
    public AudioClip fallSound;
    public AudioClip exitSound;
    public AudioClip victorySound;
    public AudioClip buttonSound;
 
    [Header("Background music")]
    public AudioClip musicClip;
 
    [Range(0f, 1f)] public float sfxVolume   = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.4f;
 
    private AudioSource sfxSource;
    private AudioSource musicSource;



    void Awake()
    {
        // Singleton pattern: keep only one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        sfxSource             = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume      = sfxVolume;
 
        musicSource             = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop        = true;
        musicSource.volume      = musicVolume;
    }

    void Start()
    {
        PlayMusic();
        LoadLevel(0);
    }

    // Called by Start and when advancing levels
    public void LoadLevel(int index)
    {
        currentLevel = index;
        lives        = levels[index].lives;
        // score between levels is accumulative

        mazeRenderer.BuildMaze(levels[index]);
        uiManager.Refresh();
    }

    // Called by Collectable.cs
    public void CollectItem(int points)
    {
        score += points;
        uiManager.Refresh();
        PlaySFX(collectSound);
    }

    // Called by HoleTrap.cs
    public void FallInHole()
    {
        lives--;
        PlaySFX(fallSound);
        uiManager.Refresh();

        if (lives <= 0)
        {
            // PlaySFX(gameOverSound);
            uiManager.ShowGameOver();
        }
        else
        {
            // Respawn ball at maze entrance, no penalty to score
            mazeRenderer.RespawnBall();
        }
    }

    // Called by ExitZone.cs
    public void ReachExit()
    {
        PlaySFX(exitSound);

        if (currentLevel + 1 >= levels.Length)
        {
            // All 5 levels beaten!
            PlaySFX(victorySound);
            uiManager.ShowVictory();
        }
        else
        {
            mazeRenderer.ClearMaze();
            LoadLevel(currentLevel + 1);
        }
    }

    // Called by UI "Restart" buttons
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // TODO: Remove later
    void Update()
    {
        // TEMP: press N to skip to next level during testing — remove before final build
        if (Input.GetKeyDown(KeyCode.N))
            ReachExit();
    }

    // Call from every UI button's OnClick
    public void PlayButtonSound()
    {
        PlaySFX(buttonSound);
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
 
    void PlayMusic()
    {
        if (musicClip == null || musicSource == null) return;
        musicSource.clip = musicClip;
        musicSource.Play();
    }
}