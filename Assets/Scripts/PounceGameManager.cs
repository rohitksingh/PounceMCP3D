using System.Collections.Generic;
using UnityEngine;

public class PounceGameManager : MonoBehaviour
{
    [Header("Win Condition")]
    public float winThreshold = 85f;

    private PlayerController _player;
    private GridRenderer _gridRenderer;
    private bool _levelComplete;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _gridRenderer = FindObjectOfType<GridRenderer>();

        _player.OnTrailComplete += HandleTrailComplete;

        // Show initial progress (starting island is ~0.2%)
        float startPct = GridManager.Instance.GetCapturedPercentage();
        HUDManager.Instance.UpdateProgress(startPct);
    }

    void OnDestroy()
    {
        if (_player != null)
            _player.OnTrailComplete -= HandleTrailComplete;
    }

    void HandleTrailComplete(List<Vector2Int> trailCells)
    {
        if (_levelComplete) return;

        FloodFill.CaptureTerritory(trailCells);
        _gridRenderer.Refresh();

        float pct = GridManager.Instance.GetCapturedPercentage();
        HUDManager.Instance.UpdateProgress(pct);

        if (pct >= winThreshold)
        {
            _levelComplete = true;
            HUDManager.Instance.ShowWin();
            Debug.Log($"Level complete! {pct:F1}% captured.");
        }
    }
}
