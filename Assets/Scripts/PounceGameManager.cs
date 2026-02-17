using System.Collections.Generic;
using UnityEngine;

public class PounceGameManager : MonoBehaviour
{
    [Header("Win Condition")]
    public float winThreshold = 85f;

    [Header("Robots")]
    public int robotCount = 5;
    public float robotSpeed = 0.8f;

    [Header("Lives")]
    public int startingLives = 3;

    private PlayerController _player;
    private GridRenderer _gridRenderer;
    private HUDManager _hud;
    private bool _levelComplete;
    private int _lives;
    private bool _isDying;

    void Start()
    {
        _player = FindFirstObjectByType<PlayerController>();
        _gridRenderer = FindFirstObjectByType<GridRenderer>();
        _hud = FindFirstObjectByType<HUDManager>();
        if (_hud == null) _hud = gameObject.AddComponent<HUDManager>();

        _player.OnTrailComplete += HandleTrailComplete;

        _lives = startingLives;
        SpawnRobots();

        float startPct = GridManager.Instance.GetCapturedPercentage();
        _hud.UpdateProgress(startPct);
        _hud.UpdateLives(_lives);
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
            robot.OnHitTrail += HandleRobotHitTrail;
        }
    }

    void HandleRobotHitTrail()
    {
        if (_levelComplete || _isDying) return;

        _lives--;
        _hud.UpdateLives(_lives);
        _isDying = true;
        _player.Respawn();
        _isDying = false;

        if (_lives <= 0)
        {
            _levelComplete = true;
            _hud.ShowGameOver();
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
