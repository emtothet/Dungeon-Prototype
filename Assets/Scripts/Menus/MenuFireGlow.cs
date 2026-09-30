using UnityEngine;
using UnityEngine.UI;

// Procedural soft glow rendered by Canvas; the edge fades to transparent.
public sealed class MenuFireGlow : MaskableGraphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = rectTransform.rect;
        const int segments = 48;
        vh.AddVert(rect.center, color, Vector2.zero);
        Color edge = color;
        edge.a = 0;
        for (int i = 0; i <= segments; i++)
        {
            float a = i * Mathf.PI * 2 / segments;
            Vector2 point = rect.center + new Vector2(Mathf.Cos(a) * rect.width / 2, Mathf.Sin(a) * rect.height / 2);
            vh.AddVert(point, edge, Vector2.zero);
            if (i > 0) vh.AddTriangle(0, i, i + 1);
        }
    }
}
