using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveDelay = 0.15f;
    public float lerpSpeed = 20f;

    // Fired when the player closes a trail back onto captured territory
    public event Action<List<Vector2Int>> OnTrailComplete;

    private int _gridX, _gridY;
    private Vector3 _targetWorldPos;
    private float _moveTimer;
    private Vector2Int _queuedDir;

    private bool _isDrawing;
    private List<Vector2Int> _trailCells = new List<Vector2Int>();

    private GridRenderer _gridRenderer;

    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        var tex = new Texture2D(8, 8, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        var fill = new Color(1f, 0.5f, 0f);
        for (int i = 0; i < 64; i++) tex.SetPixel(i % 8, i / 8, fill);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 100f);
        sr.sortingOrder = 1;
    }

    void Start()
    {
        _gridRenderer = FindFirstObjectByType<GridRenderer>();

        _gridX = GridManager.Cols / 2;
        _gridY = GridManager.Rows / 2;
        _targetWorldPos = GridManager.GridToWorld(_gridX, _gridY);
        transform.position = _targetWorldPos;
    }

    void Update()
    {
        GatherInput();
        TickMovement();
        SmoothMove();
    }

    void GatherInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if      (kb.wKey.isPressed || kb.upArrowKey.isPressed)    _queuedDir = Vector2Int.up;
        else if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  _queuedDir = Vector2Int.down;
        else if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  _queuedDir = Vector2Int.left;
        else if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) _queuedDir = Vector2Int.right;
    }

    void TickMovement()
    {
        _moveTimer -= Time.deltaTime;
        if (_moveTimer > 0f || _queuedDir == Vector2Int.zero) return;

        int nx = _gridX + _queuedDir.x;
        int ny = _gridY + _queuedDir.y;

        if (nx < 0 || nx >= GridManager.Cols || ny < 0 || ny >= GridManager.Rows) return;

        var gm = GridManager.Instance;
        CellState nextCell = gm.GetCell(nx, ny);

        // Block movement onto own trail (death handled in Step 4)
        if (nextCell == CellState.Trail) return;

        _gridX = nx;
        _gridY = ny;
        _targetWorldPos = GridManager.GridToWorld(_gridX, _gridY);
        _moveTimer = moveDelay;

        HandleTrail(nextCell, nx, ny, gm);
    }

    void HandleTrail(CellState cellState, int x, int y, GridManager gm)
    {
        if (cellState == CellState.Void)
        {
            // Step into void — mark as trail and start/continue drawing
            gm.SetCell(x, y, CellState.Trail);
            _trailCells.Add(new Vector2Int(x, y));
            _gridRenderer?.RefreshCell(x, y);
            _isDrawing = true;
        }
        else if (cellState == CellState.Captured && _isDrawing)
        {
            // Returned to captured territory — trail is complete
            _isDrawing = false;
            var completedTrail = new List<Vector2Int>(_trailCells);
            _trailCells.Clear();
            OnTrailComplete?.Invoke(completedTrail);
        }
    }

    void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, _targetWorldPos, lerpSpeed * Time.deltaTime);
    }

    public Vector2Int GridPosition => new Vector2Int(_gridX, _gridY);
    public bool IsDrawing => _isDrawing;
}
