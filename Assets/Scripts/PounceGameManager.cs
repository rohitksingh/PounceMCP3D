using System.Collections.Generic;
using UnityEngine;

public class PounceGameManager : MonoBehaviour
{
    [Header("Win Condition")]
    public float winThreshold = 85f;

    [Header("Robots")]
    public int robotCount = 5;
    public float robotSpeed = 0.8f;

    private PlayerController _player;
    private GridRenderer _gridRenderer;
    private HUDManager _hud;
    private bool _levelComplete;

    void Start()
    {
        _player = FindFirstObjectByType<PlayerController>();
        _gridRenderer = FindFirstObjectByType<GridRenderer>();
        _hud = FindFirstObjectByType<HUDManager>();
        if (_hud == null) _hud = gameObject.AddComponent<HUDManager>();

        _player.OnTrailComplete += HandleTrailComplete;

        SpawnRobots();

        float startPct = GridManager.Instance.GetCapturedPercentage();
        _hud.UpdateProgress(startPct);
    }

    void OnDestroy()
    {
        if (_player != null)
            _player.OnTrailComplete -= HandleTrailComplete;
    }

    void SpawnRobots()
    {
        for (int i = 0; i < robotCount; i++)
        {
            var go = new GameObject($"Robot_{i}");
            go.AddComponent<SpriteRenderer>();
            var robot = go.AddComponent<Robot>();
            robot.speed = robotSpeed;
        }
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
