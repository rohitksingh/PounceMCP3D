using System.Collections.Generic;
using UnityEngine;

public static class FloodFill
{
    /// <summary>
    /// Converts trail cells to Captured, then captures all enclosed Void regions.
    /// Algorithm: find all distinct void regions, keep the largest (outer sea),
    /// capture everything else. Works correctly with a captured border.
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

        // Step 2: Find all distinct void regions
        var visited = new HashSet<Vector2Int>();
        var regions = new List<List<Vector2Int>>();

        for (int x = 0; x < GridManager.Cols; x++)
        {
            for (int y = 0; y < GridManager.Rows; y++)
            {
                var pos = new Vector2Int(x, y);
                if (gm.GetCell(x, y) == CellState.Void && !visited.Contains(pos))
                    regions.Add(BfsRegion(gm, pos, visited));
            }
        }

        if (regions.Count == 0) return newlyCaptured;

        // Step 3: Keep the largest region (outer sea), capture all smaller ones
        int largestIdx = 0;
        for (int i = 1; i < regions.Count; i++)
            if (regions[i].Count > regions[largestIdx].Count)
                largestIdx = i;

        for (int i = 0; i < regions.Count; i++)
        {
            if (i == largestIdx) continue;
            foreach (var cell in regions[i])
            {
                gm.SetCell(cell.x, cell.y, CellState.Captured);
                newlyCaptured.Add(cell);
            }
        }

        return newlyCaptured;
    }

    static List<Vector2Int> BfsRegion(GridManager gm, Vector2Int start, HashSet<Vector2Int> visited)
    {
        var region = new List<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            region.Add(c);

            TryEnqueue(gm, queue, visited, c.x + 1, c.y);
            TryEnqueue(gm, queue, visited, c.x - 1, c.y);
            TryEnqueue(gm, queue, visited, c.x, c.y + 1);
            TryEnqueue(gm, queue, visited, c.x, c.y - 1);
        }

        return region;
    }

    static void TryEnqueue(GridManager gm, Queue<Vector2Int> queue,
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
