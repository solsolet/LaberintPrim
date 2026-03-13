using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    public MazeGenerator generator;

    public GameObject wallPrefab;
    public GameObject floorPrefab;

    public float tileSize = 2f;

    void Start()
    {
        int[,] maze = generator.GenerateMaze();

        int width = maze.GetLength(0);
        int height = maze.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);

                if (maze[x, y] == 0)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity);
                }
                else
                {
                    Instantiate(floorPrefab, position, Quaternion.identity);
                }
            }
        }
    }
}