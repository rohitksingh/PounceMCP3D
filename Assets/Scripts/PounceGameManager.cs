using System.Collections.Generic;
using UnityEngine;

public class PounceGameManager : MonoBehaviour
{
    [Header("Win Condition")]
    public float winThreshold = 85f;

    private PlayerController _player;
    private GridRenderer _gridRenderer;
    private HUDManager _hud;
    private bool _levelComplete;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _gridRenderer = FindObjectOfType<GridRenderer>();
        _hud = FindObjectOfType<HUDManager>();

        _player.OnTrailComplete += HandleTrailComplete;

        float startPct = GridManager.Instance.GetCapturedPercentage();
        _hud.UpdateProgress(startPct);
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
        _hud.UpdateProgress(pct);

        if (pct >= winThreshold)
        {
            _levelComplete = true;
            _hud.ShowWin();
            Debug.Log($"Level complete! {pct:F1}% captured.");
        }
    }
}
