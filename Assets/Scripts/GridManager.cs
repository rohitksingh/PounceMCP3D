using UnityEngine;

public class GridManager : MonoBehaviour
{
    public const int Cols = 80;
    public const int Rows = 60;
    public const int CellSize = 10; // pixels per cell

    // Starting island half-size (creates a 7x7 captured center block)
    private const int IslandHalf = 3;

    private CellState[,] _grid = new CellState[Cols, Rows];

    public static GridManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        InitGrid();
    }

    void InitGrid()
    {
        for (int x = 0; x < Cols; x++)
            for (int y = 0; y < Rows; y++)
                _grid[x, y] = CellState.Void;

        // Center starting island
        int cx = Cols / 2;
        int cy = Rows / 2;
        for (int x = cx - IslandHalf; x <= cx + IslandHalf; x++)
            for (int y = cy - IslandHalf; y <= cy + IslandHalf; y++)
                _grid[x, y] = CellState.Captured;
    }

    /// <summary>Returns the cell state. Out-of-bounds is treated as Captured (border wall).</summary>
    public CellState GetCell(int x, int y)
    {
        if (x < 0 || x >= Cols || y < 0 || y >= Rows)
            return CellState.Captured;
        return _grid[x, y];
    }

    public void SetCell(int x, int y, CellState state)
    {
        if (x < 0 || x >= Cols || y < 0 || y >= Rows) return;
        _grid[x, y] = state;
    }

    public float GetCapturedPercentage()
    {
        int captured = 0;
        for (int x = 0; x < Cols; x++)
            for (int y = 0; y < Rows; y++)
                if (_grid[x, y] == CellState.Captured)
                    captured++;
        return (float)captured / (Cols * Rows) * 100f;
    }
}
