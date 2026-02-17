using System.Collections.Generic;
using UnityEngine;

public static class FloodFill
{
    /// <summary>
    /// Converts trail cells to Captured, then captures all Void regions enclosed
    /// by the new boundary. Returns all newly captured cells.
    /// Algorithm: BFS from the grid border to find the "outer" void (unreachable
    /// from outside = enclosed = captured).
    /// </summary>
    public static List<Vector2Int> CaptureTerritory(List<Vector2Int> trailCells)
    {
        var gm = GridManager.Instance;
        var newlyCaptured = new List<Vector2Int>();

        // Step 1: Convert trail → Captured
        foreach (var cell in trailCells)
        {
            gm.SetCell(cell.x, cell.y, CellState.Captured);
            newlyCaptured.Add(cell);
        }

        // Step 2: BFS from every border cell to find all Void reachable from outside
        var outerVoid = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();

        for (int x = 0; x < GridManager.Cols; x++)
        {
            TryEnqueue(gm, queue, outerVoid, x, 0);
            TryEnqueue(gm, queue, outerVoid, x, GridManager.Rows - 1);
        }
        for (int y = 1; y < GridManager.Rows - 1; y++)
        {
            TryEnqueue(gm, queue, outerVoid, 0, y);
            TryEnqueue(gm, queue, outerVoid, GridManager.Cols - 1, y);
        }

        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            TryEnqueue(gm, queue, outerVoid, c.x + 1, c.y);
            TryEnqueue(gm, queue, outerVoid, c.x - 1, c.y);
            TryEnqueue(gm, queue, outerVoid, c.x, c.y + 1);
            TryEnqueue(gm, queue, outerVoid, c.x, c.y - 1);
        }

        // Step 3: Any Void cell NOT reachable from the border → enclosed → Captured
        for (int x = 0; x < GridManager.Cols; x++)
        {
            for (int y = 0; y < GridManager.Rows; y++)
            {
                if (gm.GetCell(x, y) == CellState.Void && !outerVoid.Contains(new Vector2Int(x, y)))
                {
                    gm.SetCell(x, y, CellState.Captured);
                    newlyCaptured.Add(new Vector2Int(x, y));
                }
            }
        }

        return newlyCaptured;
    }

    private static void TryEnqueue(GridManager gm, Queue<Vector2Int> queue,
        HashSet<Vector2Int> visited, int x, int y)
    {
        if (x < 0 || x >= GridManager.Cols || y < 0 || y >= GridManager.Rows) return;
        var pos = new Vector2Int(x, y);
        if (visited.Contains(pos)) return;
        if (gm.GetCell(x, y) != CellState.Void) return;
        visited.Add(pos);
        queue.Enqueue(pos);
    }
}
