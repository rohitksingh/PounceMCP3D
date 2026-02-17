using System.Collections.Generic;
using UnityEngine;

public class PounceGameManager : MonoBehaviour
{
    private PlayerController _player;
    private GridRenderer _gridRenderer;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _gridRenderer = FindObjectOfType<GridRenderer>();

        _player.OnTrailComplete += HandleTrailComplete;
    }

    void OnDestroy()
    {
        if (_player != null)
            _player.OnTrailComplete -= HandleTrailComplete;
    }

    void HandleTrailComplete(List<Vector2Int> trailCells)
    {
        FloodFill.CaptureTerritory(trailCells);
        _gridRenderer.Refresh();

        float pct = GridManager.Instance.GetCapturedPercentage();
        Debug.Log($"Territory: {pct:F1}%");
    }
}
