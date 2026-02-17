using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Robot : MonoBehaviour
{
    public float speed = 0.8f;

    private Vector2 _velocity;

    void Awake()
    {
        // Simple red square sprite, same size as player cell
        var sr = GetComponent<SpriteRenderer>();
        var tex = new Texture2D(8, 8, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        var fill = new Color(0.9f, 0.2f, 0.2f);
        for (int i = 0; i < 64; i++) tex.SetPixel(i % 8, i / 8, fill);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 100f);
        sr.sortingOrder = 1;
    }

    void Start()
    {
        SpawnInVoid();

        // Random diagonal direction
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        _velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        var gm = GridManager.Instance;
        Vector3 pos = transform.position;

        // Predict next position
        float dx = _velocity.x * Time.deltaTime;
        float dy = _velocity.y * Time.deltaTime;

        // Check X axis independently
        var xCell = GridManager.WorldToGrid(new Vector3(pos.x + dx, pos.y));
        if (IsBlocked(xCell, gm))
        {
            _velocity.x = -_velocity.x;
            dx = 0f;
        }

        // Check Y axis independently
        var yCell = GridManager.WorldToGrid(new Vector3(pos.x, pos.y + dy));
        if (IsBlocked(yCell, gm))
        {
            _velocity.y = -_velocity.y;
            dy = 0f;
        }

        transform.position = new Vector3(pos.x + dx, pos.y + dy, pos.z);
    }

    bool IsBlocked(Vector2Int cell, GridManager gm)
    {
        // Out of bounds = blocked
        if (cell.x < 0 || cell.x >= GridManager.Cols ||
            cell.y < 0 || cell.y >= GridManager.Rows) return true;

        return gm.GetCell(cell.x, cell.y) == CellState.Captured;
    }

    void SpawnInVoid()
    {
        var gm = GridManager.Instance;
        for (int attempt = 0; attempt < 200; attempt++)
        {
            int x = Random.Range(5, GridManager.Cols - 5);
            int y = Random.Range(5, GridManager.Rows - 5);
            if (gm.GetCell(x, y) == CellState.Void)
            {
                transform.position = GridManager.GridToWorld(x, y);
                return;
            }
        }
    }
}
