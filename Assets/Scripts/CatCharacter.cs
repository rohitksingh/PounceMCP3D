using UnityEngine;

/// <summary>
/// Builds a 3D cat character from Unity primitives at runtime.
/// Attach to the Player GameObject — it replaces the flat sprite.
/// </summary>
public class CatCharacter : MonoBehaviour
{
    [Header("Size")]
    public float scale = 4f;

    [Header("Colors")]
    public Color bodyColor     = new Color(1f,    0.55f, 0.15f);
    public Color earInnerColor = new Color(1f,    0.75f, 0.75f);
    public Color eyeColor      = new Color(0.15f, 0.80f, 0.35f);
    public Color pupilColor    = new Color(0.05f, 0.05f, 0.05f);
    public Color noseColor     = new Color(0.85f, 0.40f, 0.50f);

    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false;

        BuildCat();
    }

    void BuildCat()
    {
        // Apply scale on this transform — all children inherit it
        transform.localScale = Vector3.one * scale;

        // ── Body ──────────────────────────────────────────────────────────
        Part("Body", PrimitiveType.Sphere, Vector3.zero,
            new Vector3(0.18f, 0.10f, 0.14f), bodyColor, 0.3f);

        // ── Head ──────────────────────────────────────────────────────────
        Part("Head", PrimitiveType.Sphere,
            new Vector3(0.14f, 0.06f, 0f),
            new Vector3(0.12f, 0.11f, 0.12f), bodyColor, 0.3f);

        // ── Ears ──────────────────────────────────────────────────────────
        var earL = Part("EarLeft", PrimitiveType.Cube,
            new Vector3(0.17f, 0.14f,  0.055f),
            new Vector3(0.04f, 0.07f,  0.04f), bodyColor, 0.2f);
        earL.transform.localRotation = Quaternion.Euler(0f, 0f, 15f);

        var earR = Part("EarRight", PrimitiveType.Cube,
            new Vector3(0.17f, 0.14f, -0.055f),
            new Vector3(0.04f, 0.07f,  0.04f), bodyColor, 0.2f);
        earR.transform.localRotation = Quaternion.Euler(0f, 0f, -15f);

        Part("EarInnerLeft", PrimitiveType.Cube,
            new Vector3(0.175f, 0.14f,  0.055f),
            new Vector3(0.022f, 0.045f, 0.022f), earInnerColor, 0.2f);
        Part("EarInnerRight", PrimitiveType.Cube,
            new Vector3(0.175f, 0.14f, -0.055f),
            new Vector3(0.022f, 0.045f, 0.022f), earInnerColor, 0.2f);

        // ── Eyes ──────────────────────────────────────────────────────────
        Part("EyeLeft",  PrimitiveType.Sphere, new Vector3(0.20f, 0.11f,  0.045f), new Vector3(0.030f, 0.030f, 0.020f), eyeColor,   0.9f);
        Part("EyeRight", PrimitiveType.Sphere, new Vector3(0.20f, 0.11f, -0.045f), new Vector3(0.030f, 0.030f, 0.020f), eyeColor,   0.9f);
        Part("PupilLeft",  PrimitiveType.Sphere, new Vector3(0.205f, 0.112f,  0.045f), new Vector3(0.014f, 0.022f, 0.010f), pupilColor, 0.5f);
        Part("PupilRight", PrimitiveType.Sphere, new Vector3(0.205f, 0.112f, -0.045f), new Vector3(0.014f, 0.022f, 0.010f), pupilColor, 0.5f);

        // ── Nose ──────────────────────────────────────────────────────────
        Part("Nose", PrimitiveType.Sphere,
            new Vector3(0.225f, 0.09f, 0f),
            new Vector3(0.018f, 0.012f, 0.012f), noseColor, 0.6f);

        // ── Tail ──────────────────────────────────────────────────────────
        var tail = Part("Tail", PrimitiveType.Capsule,
            new Vector3(-0.14f, 0.06f, 0.10f),
            new Vector3(0.030f, 0.11f, 0.030f), bodyColor, 0.25f);
        tail.transform.localRotation = Quaternion.Euler(0f, 30f, 55f);

        Part("TailTip", PrimitiveType.Sphere,
            new Vector3(-0.20f, 0.16f, 0.13f),
            new Vector3(0.045f, 0.045f, 0.045f),
            new Color(bodyColor.r * 0.8f, bodyColor.g * 0.6f, bodyColor.b * 0.3f), 0.25f);

        // ── Legs ──────────────────────────────────────────────────────────
        Part("LegFL", PrimitiveType.Capsule, new Vector3( 0.08f, -0.06f,  0.07f), new Vector3(0.028f, 0.055f, 0.028f), bodyColor, 0.2f);
        Part("LegFR", PrimitiveType.Capsule, new Vector3( 0.08f, -0.06f, -0.07f), new Vector3(0.028f, 0.055f, 0.028f), bodyColor, 0.2f);
        Part("LegBL", PrimitiveType.Capsule, new Vector3(-0.08f, -0.06f,  0.07f), new Vector3(0.028f, 0.055f, 0.028f), bodyColor, 0.2f);
        Part("LegBR", PrimitiveType.Capsule, new Vector3(-0.08f, -0.06f, -0.07f), new Vector3(0.028f, 0.055f, 0.028f), bodyColor, 0.2f);
    }

    GameObject Part(string partName, PrimitiveType type,
        Vector3 localPos, Vector3 localScale, Color color, float smoothness)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = partName;
        go.transform.SetParent(transform, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = localScale;

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Smoothness", smoothness);
        go.GetComponent<Renderer>().material = mat;

        var col = go.GetComponent<Collider>();
        if (col) Destroy(col);

        return go;
    }
}
