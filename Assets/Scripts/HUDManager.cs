using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    private Image _progressFill;
    private Text _percentageText;
    private Text _winText;

    void Awake()
    {
        Instance = this;
        BuildHUD();
    }

    void BuildHUD()
    {
        // --- Canvas ---
        var canvasGO = new GameObject("HUDCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);
        canvasGO.AddComponent<GraphicRaycaster>();

        // --- Progress bar background (bottom of screen) ---
        var bgGO = new GameObject("ProgressBG");
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.08f, 0.08f, 0.08f, 0.85f);
        var bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0f);
        bgRect.anchorMax = new Vector2(1f, 0f);
        bgRect.pivot    = new Vector2(0.5f, 0f);
        bgRect.offsetMin = new Vector2(10f, 8f);
        bgRect.offsetMax = new Vector2(-10f, 28f);

        // --- Progress fill ---
        var fillGO = new GameObject("ProgressFill");
        fillGO.transform.SetParent(bgGO.transform, false);
        _progressFill = fillGO.AddComponent<Image>();
        _progressFill.color = new Color(0.31f, 0.65f, 0.37f);
        _progressFill.type = Image.Type.Filled;
        _progressFill.fillMethod = Image.FillMethod.Horizontal;
        _progressFill.fillAmount = 0f;
        var fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2f, 2f);
        fillRect.offsetMax = new Vector2(-2f, -2f);

        // --- Percentage text (right side) ---
        var textGO = new GameObject("PercentText");
        textGO.transform.SetParent(canvasGO.transform, false);
        _percentageText = textGO.AddComponent<Text>();
        _percentageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _percentageText.fontSize = 14;
        _percentageText.color = Color.white;
        _percentageText.alignment = TextAnchor.MiddleRight;
        _percentageText.text = "0.0%";
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(1f, 0f);
        textRect.anchorMax = new Vector2(1f, 0f);
        textRect.pivot     = new Vector2(1f, 0f);
        textRect.offsetMin = new Vector2(-80f, 30f);
        textRect.offsetMax = new Vector2(-10f, 50f);

        // --- Win message (hidden by default) ---
        var winGO = new GameObject("WinText");
        winGO.transform.SetParent(canvasGO.transform, false);
        _winText = winGO.AddComponent<Text>();
        _winText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _winText.fontSize = 48;
        _winText.fontStyle = FontStyle.Bold;
        _winText.color = new Color(0.95f, 0.77f, 0.06f);
        _winText.alignment = TextAnchor.MiddleCenter;
        _winText.text = "LEVEL CLEAR!";
        _winText.gameObject.SetActive(false);
        var winRect = winGO.GetComponent<RectTransform>();
        winRect.anchorMin = new Vector2(0.5f, 0.5f);
        winRect.anchorMax = new Vector2(0.5f, 0.5f);
        winRect.pivot     = new Vector2(0.5f, 0.5f);
        winRect.sizeDelta = new Vector2(400f, 80f);
    }

    public void UpdateProgress(float percentage)
    {
        _progressFill.fillAmount = percentage / 100f;
        _percentageText.text = $"{percentage:F1}%";
    }

    public void ShowWin()
    {
        _winText.gameObject.SetActive(true);
    }
}
