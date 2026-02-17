using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveDelay = 0.15f;  // seconds between grid steps
    public float lerpSpeed = 20f;    // visual smoothing speed

    private int _gridX, _gridY;
    private Vector3 _targetWorldPos;
    private float _moveTimer;
    private Vector2Int _queuedDir;

    void Awake()
    {
        // Build a simple 8x8 orange square sprite at runtime (no asset needed)
        var sr = GetComponent<SpriteRenderer>();
        var tex = new Texture2D(8, 8, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        var fill = new Color(1f, 0.5f, 0f);
        for (int i = 0; i < 8 * 8; i++) tex.SetPixel(i % 8, i / 8, fill);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 100f);
        sr.sortingOrder = 1; // render above grid
    }

    void Start()
    {
        // Spawn at center of the starting island
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
        if      (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    _queuedDir = Vector2Int.up;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  _queuedDir = Vector2Int.down;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  _queuedDir = Vector2Int.left;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) _queuedDir = Vector2Int.right;
    }

    void TickMovement()
    {
        _moveTimer -= Time.deltaTime;
        if (_moveTimer > 0f || _queuedDir == Vector2Int.zero) return;

        int nx = _gridX + _queuedDir.x;
        int ny = _gridY + _queuedDir.y;

        // Clamp to grid bounds
        if (nx < 0 || nx >= GridManager.Cols || ny < 0 || ny >= GridManager.Rows) return;

        _gridX = nx;
        _gridY = ny;
        _targetWorldPos = GridManager.GridToWorld(_gridX, _gridY);
        _moveTimer = moveDelay;
    }

    void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, _targetWorldPos, lerpSpeed * Time.deltaTime);
    }

    public Vector2Int GridPosition => new Vector2Int(_gridX, _gridY);
}
