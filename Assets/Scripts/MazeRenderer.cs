using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    // ---------- Prefab references (assign in Inspector) ----------
    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject holePrefab;
    public GameObject exitPrefab;
    public GameObject ballPrefab;
    public GameObject collectableSmallPrefab;
    public GameObject collectableMediumPrefab;
    public GameObject collectableLargePrefab;

    [Header("Settings")]
    public float tileSize = 1f;   // Must match prefab dimensions

    // ---------- Private state ----------
    private int[,] maze;
    private int mazeWidth, mazeHeight;
    private GameObject ballInstance;
    private Vector3 ballSpawnPos;
    private Transform mazeParent;           // All maze objects live under this
    private List<GameObject> spawnedItems   // Holes, collectables, exit, ball
        = new List<GameObject>();

    // ---------- MazeGenerator reference ----------
    private MazeGenerator generator;

    void Awake()
    {
        generator = GetComponent<MazeGenerator>();
        if (generator == null)
            generator = gameObject.AddComponent<MazeGenerator>();
    }

    // Called by GameManager.LoadLevel()
    public void BuildMaze(GameManager.LevelConfig config)
    {
        ClearMaze(); // Remove previous level if any

        // Create a parent object to keep the hierarchy tidy
        mazeParent = new GameObject("Maze").transform;

        // Enforce odd dimensions
        mazeWidth  = (config.mazeWidth  % 2 == 0) ? config.mazeWidth  + 1 : config.mazeWidth;
        mazeHeight = (config.mazeHeight % 2 == 0) ? config.mazeHeight + 1 : config.mazeHeight;

        maze = generator.Generate(mazeWidth, mazeHeight);

        // ---- Collect available floor positions ----
        // We'll randomly assign holes and collectables to these
        List<Vector2Int> freeTiles = new List<Vector2Int>();

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, 0f, y * tileSize);

                if (maze[x, y] == 0)
                {
                    // Wall tile
                    Instantiate(wallPrefab, pos, Quaternion.identity, mazeParent);
                }
                else
                {
                    // Floor tile
                    Instantiate(floorPrefab, pos, Quaternion.identity, mazeParent);

                    // Exclude entrance (1,1) and exit cell from random placement
                    bool isEntrance = (x == 1 && y == 1);
                    bool isExit     = (x == mazeWidth - 2 && y == mazeHeight - 2);

                    if (!isEntrance && !isExit)
                        freeTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        // ---- Place exit marker ----
        Vector3 exitPos = new Vector3((mazeWidth - 2) * tileSize, 0.05f, (mazeHeight - 2) * tileSize);
        var exitObj = Instantiate(exitPrefab, exitPos, Quaternion.identity, mazeParent);
        spawnedItems.Add(exitObj);

        // ---- Shuffle free tiles ----
        Shuffle(freeTiles);
        int idx = 0;

        // ---- Place holes ----
        for (int i = 0; i < config.holeCount && idx < freeTiles.Count; i++, idx++)
        {
            Vector3 hPos = new Vector3(freeTiles[idx].x * tileSize, 0.05f,
                                        freeTiles[idx].y * tileSize);
            spawnedItems.Add(Instantiate(holePrefab, hPos, Quaternion.identity, mazeParent));
        }

        // ---- Place collectables ----
        PlaceCollectables(collectableSmallPrefab,  config.smallCollectables,  freeTiles, ref idx);
        PlaceCollectables(collectableMediumPrefab, config.mediumCollectables, freeTiles, ref idx);
        PlaceCollectables(collectableLargePrefab,  config.largeCollectables,  freeTiles, ref idx);

        // ---- Spawn ball at entrance ----
        ballSpawnPos = new Vector3(1 * tileSize, 1f, 1 * tileSize);
        ballInstance = Instantiate(ballPrefab, ballSpawnPos, Quaternion.identity);
        spawnedItems.Add(ballInstance);

        // ---- Aim camera at maze centre ----
        PositionCamera();
    }

    // Place a number of collectables of one type
    void PlaceCollectables(GameObject prefab, int count,
                           List<Vector2Int> tiles, ref int idx)
    {
        for (int i = 0; i < count && idx < tiles.Count; i++, idx++)
        {
            Vector3 pos = new Vector3(tiles[idx].x * tileSize, 0.5f,
                                       tiles[idx].y * tileSize);
            spawnedItems.Add(Instantiate(prefab, pos, Quaternion.identity, mazeParent));
        }
    }

    // Respawn ball at entrance (called after losing a life)
    public void RespawnBall()
    {
        if (ballInstance == null) return;

        ballInstance.transform.position = ballSpawnPos;

        var rb = ballInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // Destroy all maze objects (called before loading next level)
    public void ClearMaze()
    {
        foreach (var obj in spawnedItems)
            if (obj != null) Destroy(obj);

        spawnedItems.Clear();
        ballInstance = null;

        if (mazeParent != null)
            Destroy(mazeParent.gameObject);
    }

    // Fisher-Yates shuffle
    void Shuffle(List<Vector2Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // Move the main camera to look down at the maze
    void PositionCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float cx = (mazeWidth  - 1) * tileSize / 2f;
        float cz = (mazeHeight - 1) * tileSize / 2f;
        float dist = Mathf.Max(mazeWidth, mazeHeight) * tileSize;

        cam.transform.position = new Vector3(cx, dist * 0.85f, cz - dist * 0.15f);
        cam.transform.LookAt(new Vector3(cx, 0, cz));
    }
}