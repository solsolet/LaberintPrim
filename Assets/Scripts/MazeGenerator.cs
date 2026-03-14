using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    // Generates a maze as a 2D int array.
    // 0 = wall, 1 = floor/path
    // Entrance is always at (1,1)
    // Exit cell is always at (width-2, height-2)

    public int[,] Generate(int width, int height)
    {
        // Prim's algorithm requires odd dimensions
        if (width  % 2 == 0) width++;
        if (height % 2 == 0) height++;

        int[,] maze = new int[width, height];
        // Everything starts as wall (0) — no need to fill, C# initialises to 0

        // --- Start Prim from (1,1) ---
        maze[1, 1] = 1;

        List<Vector2Int> frontier = new List<Vector2Int>();
        AddFrontier(maze, frontier, 1, 1, width, height);

        while (frontier.Count > 0)
        {
            // Pick a random frontier cell
            int idx  = Random.Range(0, frontier.Count);
            Vector2Int cell = frontier[idx];
            frontier.RemoveAt(idx);

            // Get its already-visited neighbours (2 steps away)
            List<Vector2Int> visited = GetVisitedNeighbors(maze, cell, width, height);

            // Only connect if exactly one visited neighbour (Prim's condition)
            if (visited.Count >= 1)
            {
                // Pick one visited neighbour at random to connect to
                Vector2Int neighbor = visited[Random.Range(0, visited.Count)];

                // Carve: mark the cell and the wall between them as floor
                int midX = (cell.x + neighbor.x) / 2;
                int midY = (cell.y + neighbor.y) / 2;
                maze[cell.x, cell.y] = 1;
                maze[midX, midY]     = 1;

                // Expand frontier from the newly carved cell
                AddFrontier(maze, frontier, cell.x, cell.y, width, height);
            }
        }

        // --- Guarantee exit cell is open ---
        int ex = width  - 2;
        int ey = height - 2;
        maze[ex, ey] = 1;

        // Make sure at least one neighbour of the exit is also open so it's reachable
        if (maze[ex - 1, ey] == 0 && maze[ex, ey - 1] == 0)
            maze[ex - 1, ey] = 1; // open the west wall

        return maze;
    }

    // -------------------------------------------------------
    // Adds unvisited cells 2 steps away to the frontier list
    private void AddFrontier(int[,] maze, List<Vector2Int> frontier,
                              int x, int y, int width, int height)
    {
        int[,] dirs = { {2,0},{-2,0},{0,2},{0,-2} };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dirs[i, 0];
            int ny = y + dirs[i, 1];

            // Must be inside the inner grid (not on the border ring)
            if (nx > 0 && ny > 0 && nx < width - 1 && ny < height - 1 && maze[nx, ny] == 0)
            {
                var v = new Vector2Int(nx, ny);
                if (!frontier.Contains(v))
                    frontier.Add(v);
            }
        }
    }

    // Returns already-visited cells 2 steps away (value == 1)
    private List<Vector2Int> GetVisitedNeighbors(int[,] maze, Vector2Int cell,
                                                  int width, int height)
    {
        var result = new List<Vector2Int>();
        int[,] dirs = { {2,0},{-2,0},{0,2},{0,-2} };

        for (int i = 0; i < 4; i++)
        {
            int nx = cell.x + dirs[i, 0];
            int ny = cell.y + dirs[i, 1];

            if (nx > 0 && ny > 0 && nx < width - 1 && ny < height - 1 && maze[nx, ny] == 1)
                result.Add(new Vector2Int(nx, ny));
        }

        return result;
    }
}