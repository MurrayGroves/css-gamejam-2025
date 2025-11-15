// csharp
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class ChunkHeightGizmo
{
    private const float MaxHeight = 16f;

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
    private static void Draw(ChunkMetadata meta, GizmoType gizmoType)
    {
        var tilemap = meta ? meta.GetComponent<Tilemap>() : null;
        if (!tilemap) return;

        tilemap.CompressBounds();
        var cb = tilemap.cellBounds;
        if (cb.size.x == 0 && cb.size.y == 0) return;

        // X span from current content; Y band anchored to world Y=0
        var leftAtAnyY = tilemap.CellToWorld(new Vector3Int(cb.xMin, cb.yMin, 0));
        var rightAtAnyY = tilemap.CellToWorld(new Vector3Int(cb.xMax, cb.yMin, 0));
        var topAtAnyX = tilemap.CellToWorld(new Vector3Int(cb.xMin, cb.yMax, 0));

        float leftX = leftAtAnyY.x;
        float rightX = rightAtAnyY.x;

        float bottomY = 0f;               // absolute baseline
        float topLimitY = bottomY + MaxHeight;

        float currentTopY = topAtAnyX.y;
        bool tooTall = currentTopY > topLimitY + 1e-4f;

        float selBoost = (gizmoType & GizmoType.Selected) != 0 ? 0.06f : 0f;
        var fill = tooTall ? new Color(1f, 0f, 0f, 0.08f + selBoost) : new Color(0f, 1f, 0f, 0.06f + selBoost);
        var outline = tooTall ? new Color(1f, 0f, 0f, 0.9f) : new Color(0f, 0.8f, 0f, 0.9f);
        var line = tooTall ? new Color(1f, 0.2f, 0.2f, 1f) : new Color(0.2f, 1f, 0.2f, 1f);

        Vector3 a = new(leftX, bottomY, 0f);
        Vector3 b = new(rightX, bottomY, 0f);
        Vector3 c = new(rightX, topLimitY, 0f);
        Vector3 d = new(leftX, topLimitY, 0f);

        Handles.DrawSolidRectangleWithOutline(new[] { a, b, c, d }, fill, outline);

        float pad = Mathf.Max(0.5f, (rightX - leftX) * 0.02f);
        Handles.color = line;
        // 0 baseline
        Handles.DrawLine(new Vector3(leftX - pad, bottomY, 0f), new Vector3(rightX + pad, bottomY, 0f));
        // 16u limit line
        Handles.DrawLine(new Vector3(leftX - pad, topLimitY, 0f), new Vector3(rightX + pad, topLimitY, 0f));

        var style = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = outline },
            alignment = TextAnchor.MiddleCenter
        };
        Handles.Label(new Vector3((leftX + rightX) * 0.5f, topLimitY, 0f) + Vector3.up * 0.25f,
            $"Band 0..{MaxHeight:0}u • Top Y {currentTopY:0.##}u", style);
    }
}
