#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace _Game.Data
{
    public partial class ShapeDefinition
    {
        private static readonly Color FilledColor = new(0.20f, 0.75f, 0.35f);
        private static readonly Color EmptyColor = new(0.18f, 0.18f, 0.18f);
        private const float CellPadding = 1f;

        private static bool DrawCell(Rect rect, bool value)
        {
            if (Event.current.type == EventType.MouseDown &&
                Event.current.button == 0 &&
                rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                GUI.changed = true;
                Event.current.Use();
            }

            EditorGUI.DrawRect(Inset(rect, CellPadding), value ? FilledColor : EmptyColor);
            return value;
        }

        private static Rect Inset(Rect rect, float padding) =>
            new(rect.x + padding, rect.y + padding,
                rect.width - padding * 2f, rect.height - padding * 2f);
    }
}
#endif
