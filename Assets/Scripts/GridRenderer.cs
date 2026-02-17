using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GridRenderer : MonoBehaviour
{
    // Pixels per Unity unit — grid will be 8x6 world units
    private const float PixelsPerUnit = 100f;

    private static readonly Color VoidColor     = new Color(0.12f, 0.23f, 0.37f); // deep blue
    private static readonly Color CapturedColor = new Color(0.31f, 0.65f, 0.37f); // green
    private static readonly Color TrailColor    = new Color(0.95f, 0.77f, 0.06f); // golden yellow

    private Texture2D _texture;
    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();

        int texW = GridManager.Cols * GridManager.CellSize;
        int texH = GridManager.Rows * GridManager.CellSize;

        _texture = new Texture2D(texW, texH, TextureFormat.RGB24, false);
        _texture.filterMode = FilterMode.Point;
        _texture.name = "GridTexture";
    }

    void Start()
    {
        Refresh();
    }

    /// <summary>Redraws the entire grid texture. Call after any grid state change.</summary>
    public void Refresh()
    {
        var gm = GridManager.Instance;
        int texW = GridManager.Cols * GridManager.CellSize;
        int texH = GridManager.Rows * GridManager.CellSize;

        Color[] pixels = new Color[texW * texH];

        for (int x = 0; x < GridManager.Cols; x++)
        {
            for (int y = 0; y < GridManager.Rows; y++)
            {
                Color c = StateToColor(gm.GetCell(x, y));

                for (int px = 0; px < GridManager.CellSize; px++)
                {
                    for (int py = 0; py < GridManager.CellSize; py++)
                    {
                        int pixelX = x * GridManager.CellSize + px;
                        int pixelY = y * GridManager.CellSize + py;
                        pixels[pixelY * texW + pixelX] = c;
                    }
                }
            }
        }

        _texture.SetPixels(pixels);
        _texture.Apply();

        _sr.sprite = Sprite.Create(
            _texture,
            new Rect(0, 0, texW, texH),
            new Vector2(0.5f, 0.5f),
            PixelsPerUnit
        );
    }

    /// <summary>Updates a single cell on the texture (cheaper than full Refresh).</summary>
    public void RefreshCell(int x, int y)
    {
        Color c = StateToColor(GridManager.Instance.GetCell(x, y));
        int texW = GridManager.Cols * GridManager.CellSize;

        for (int px = 0; px < GridManager.CellSize; px++)
        {
            for (int py = 0; py < GridManager.CellSize; py++)
            {
                _texture.SetPixel(
                    x * GridManager.CellSize + px,
                    y * GridManager.CellSize + py,
                    c
                );
            }
        }
        _texture.Apply();
    }

    private static Color StateToColor(CellState state) => state switch
    {
        CellState.Captured => CapturedColor,
        CellState.Trail    => TrailColor,
        _                  => VoidColor
    };
}
