#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using _Game.Enums;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _Game.Data
{
    public partial class LevelData
    {
        private static LevelData _drawing;
        private static int _cursor;

        private ShapeColor[,] _overlayShape;
        private bool[,] _overlayConflict;

        private static readonly List<Vector2Int> CellBuffer = new();
        private readonly StringBuilder _issuesBuilder = new();
        private string _issues;

        private static readonly Color FillableColor = new(0.55f, 0.55f, 0.55f);
        private static readonly Color WallColor = new(0.12f, 0.12f, 0.14f);
        private static readonly Color EmptyColor = new(0.22f, 0.22f, 0.22f);
        private static readonly Color ConflictColor = new(0.85f, 0.18f, 0.18f);
        private const float BasePadding = 1f;
        private const float OverlayPadding = 4f;

        private bool HasIssues => !string.IsNullOrEmpty(_issues);
        
        [OnInspectorGUI, PropertyOrder(1.5f)]
        [InfoBox("$_issues", InfoMessageType.Warning, VisibleIf = nameof(HasIssues))]
        private void PrepareOverlay()
        {
            _drawing = this;
            _cursor = 0;
            BuildOverlay();
        }

        private void BuildOverlay()
        {
            int w = Width, h = Height;

            if (_overlayShape == null || _overlayShape.GetLength(0) != w || _overlayShape.GetLength(1) != h)
            {
                _overlayShape = new ShapeColor[w, h];
                _overlayConflict = new bool[w, h];
            }
            else
            {
                for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    _overlayShape[x, y] = ShapeColor.None;
                    _overlayConflict[x, y] = false;
                }
            }

            _issuesBuilder.Clear();

            if (shapes != null)
            {
                for (int i = 0; i < shapes.Count; i++)
                {
                    var p = shapes[i];
                    if (p?.definition == null)
                        continue;

                    p.definition.CollectCells(p.anchor, CellBuffer);

                    for (int c = 0; c < CellBuffer.Count; c++)
                    {
                        var cell = CellBuffer[c];

                        if (cell.x < 0 || cell.y < 0 || cell.x >= w || cell.y >= h)
                        {
                            _issuesBuilder.AppendLine($"Shape #{i} ({p.color}) off-board at ({cell.x},{cell.y}).");
                            continue;
                        }

                        if (cells[cell.x, cell.y] != CellType.Fillable)
                        {
                            _overlayConflict[cell.x, cell.y] = true;
                            _issuesBuilder.AppendLine($"Shape #{i} ({p.color}) on a non-fillable cell ({cell.x},{cell.y}).");
                        }

                        if (_overlayShape[cell.x, cell.y] != ShapeColor.None)
                        {
                            _overlayConflict[cell.x, cell.y] = true;
                            _issuesBuilder.AppendLine($"Overlap at ({cell.x},{cell.y}).");
                        }

                        _overlayShape[cell.x, cell.y] = p.color;
                    }
                }
            }

            _issues = _issuesBuilder.Length > 0 ? _issuesBuilder.ToString() : null;
        }

        private static CellType DrawCell(Rect rect, CellType value)
        {
            var self = _drawing;
            int h = self != null ? self.Height : 0;

            int x = 0, y = 0;
            if (h > 0)
            {
                int idx = _cursor++;
                x = idx / h;
                y = idx % h;
            }

            if (self != null &&
                Event.current.type == EventType.MouseDown &&
                Event.current.button == 0 &&
                rect.Contains(Event.current.mousePosition))
            {
                value = NextType(value);
                GUI.changed = true;
                Event.current.Use();
            }

            EditorGUI.DrawRect(Inset(rect, BasePadding), BoardColor(value));

            if (self != null && x < self.Width && y < self.Height)
            {
                if (self._overlayConflict[x, y])
                    EditorGUI.DrawRect(Inset(rect, OverlayPadding), ConflictColor);
                else if (self._overlayShape[x, y] != ShapeColor.None)
                    EditorGUI.DrawRect(Inset(rect, OverlayPadding), ShapeToColor(self._overlayShape[x, y]));
            }

            return value;
        }

        private static CellType NextType(CellType type) => type switch
        {
            CellType.Empty => CellType.Fillable,
            CellType.Fillable => CellType.Wall,
            _ => CellType.Empty
        };

        private static Color BoardColor(CellType type) => type switch
        {
            CellType.Fillable => FillableColor,
            CellType.Wall => WallColor,
            _ => EmptyColor
        };

        private static Color ShapeToColor(ShapeColor color) => color switch
        {
            ShapeColor.Red => new Color(0.85f, 0.25f, 0.25f),
            ShapeColor.Blue => new Color(0.25f, 0.45f, 0.85f),
            ShapeColor.Green => new Color(0.25f, 0.75f, 0.35f),
            ShapeColor.Yellow => new Color(0.90f, 0.80f, 0.20f),
            ShapeColor.Orange => new Color(0.95f, 0.55f, 0.20f),
            ShapeColor.Purple => new Color(0.65f, 0.35f, 0.80f),
            _ => Color.gray
        };

        private static Rect Inset(Rect rect, float padding) =>
            new(rect.x + padding, rect.y + padding,
                rect.width - padding * 2f, rect.height - padding * 2f);
    }
}
#endif
