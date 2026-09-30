using UnityEngine;
using UnityEngine.UI;

// Lightweight ambient motion over a still image; no video, package or shader required.
public sealed class MenuAtmosphere : MonoBehaviour
{
    public const string PreferenceKey = "Dungeon.Menu.Animation";
    private RectTransform background;
    private float elapsed;
    private readonly RectTransform[] embers = new RectTransform[26];
    private readonly Image[] images = new Image[26];
    private readonly float[] phases = new float[26];
    private readonly float[] speeds = new float[26];
    private MenuFireGlow glow;

    private void Start()
    {
        background = (RectTransform)transform;
        // A little overscan prevents movement from exposing the screen edge.
        background.localScale = Vector3.one * 1.035f;
        var random = new System.Random(1729);
        for (int i = 0; i < embers.Length; i++)
        {
            var obj = new GameObject("Ember", typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(transform, false);
            embers[i] = obj.GetComponent<RectTransform>();
            images[i] = obj.GetComponent<Image>();
            images[i].raycastTarget = false;
            // Keep particles over architecture and fire, away from the menu text.
            embers[i].anchorMin = embers[i].anchorMax = new Vector2(0.43f + (float)random.NextDouble() * 0.28f, 0.22f);
            float size = 1.2f + (float)random.NextDouble() * 2.2f;
            embers[i].sizeDelta = new Vector2(size, size * 1.8f);
            embers[i].localRotation = Quaternion.Euler(0, 0, 25);
            phases[i] = (float)random.NextDouble();
            speeds[i] = 0.035f + (float)random.NextDouble() * 0.045f;
        }
        var light = new GameObject("Warm Firelight", typeof(RectTransform), typeof(MenuFireGlow));
        light.transform.SetParent(transform, false);
        var rect = light.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.52f, 0.44f);
        rect.sizeDelta = new Vector2(460, 490);
        glow = light.GetComponent<MenuFireGlow>();
        glow.raycastTarget = false;
    }

    private void Update()
    {
        if (background == null) return;
        elapsed += Time.unscaledDeltaTime;
        background.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.065f) * 7f, Mathf.Sin(elapsed * 0.043f) * 4f);
        for (int i = 0; i < embers.Length; i++)
        {
            float t = Mathf.Repeat(phases[i] + elapsed * speeds[i], 1f);
            embers[i].anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.65f + i) * 12f + t * 32f, t * 390f);
            images[i].color = new Color(1f, 0.43f, 0.13f, Mathf.Sin(t * Mathf.PI) * 0.65f);
        }
        float flicker = Mathf.PerlinNoise(elapsed * 1.6f, 0.7f);
        glow.color = new Color(1f, 0.30f, 0.04f, 0.025f + flicker * 0.04f);
    }
}
