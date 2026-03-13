using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;

    private int[,] maze;

    private List<Vector2Int> frontier = new List<Vector2Int>();

    public int[,] GenerateMaze()
    {
        maze = new int[width, height];

        // inicialitzar tot com mur
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 0;
            }
        }

        // cel·la inicial
        int startX = Random.Range(1, width - 1);
        int startY = Random.Range(1, height - 1);

        maze[startX, startY] = 1;

        AddFrontier(startX, startY);

        while (frontier.Count > 0)
        {
            int randIndex = Random.Range(0, frontier.Count);
            Vector2Int cell = frontier[randIndex];
            frontier.RemoveAt(randIndex);

            List<Vector2Int> neighbors = GetVisitedNeighbors(cell);

            if (neighbors.Count == 1)
            {
                Vector2Int neighbor = neighbors[0];

                int wallX = (cell.x + neighbor.x) / 2;
                int wallY = (cell.y + neighbor.y) / 2;

                maze[cell.x, cell.y] = 1;
                maze[wallX, wallY] = 1;

                AddFrontier(cell.x, cell.y);
            }
        }

        return maze;
    }

    void AddFrontier(int x, int y)
    {
        Vector2Int[] directions =
        {
            new Vector2Int(2,0),
            new Vector2Int(-2,0),
            new Vector2Int(0,2),
            new Vector2Int(0,-2)
        };

        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (IsInside(nx, ny) && maze[nx, ny] == 0)
            {
                Vector2Int newCell = new Vector2Int(nx, ny);

                if (!frontier.Contains(newCell))
                    frontier.Add(newCell);
            }
        }
    }

    List<Vector2Int> GetVisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions =
        {
            new Vector2Int(2,0),
            new Vector2Int(-2,0),
            new Vector2Int(0,2),
            new Vector2Int(0,-2)
        };

        foreach (var dir in directions)
        {
            int nx = cell.x + dir.x;
            int ny = cell.y + dir.y;

            if (IsInside(nx, ny) && maze[nx, ny] == 1)
            {
                neighbors.Add(new Vector2Int(nx, ny));
            }
        }

        return neighbors;
    }

    bool IsInside(int x, int y)
    {
        return x > 0 && y > 0 && x < width - 1 && y < height - 1;
    }
}