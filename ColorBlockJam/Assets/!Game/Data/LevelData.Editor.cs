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

        private int[,] _overlayShapeIndex;
        private bool[,] _overlayConflict;
        private static int _draggingShapeIndex = -1;
        private static Vector2Int _dragOffset;

        private static readonly List<Vector2Int> CellBuffer = new();
        private readonly StringBuilder _issuesBuilder = new();
        private string _issues;

        private static readonly Color FillableColor = new(0.55f, 0.55f, 0.55f);
        private static readonly Color BlockColor = new(0.35f, 0.35f, 0.38f);
        private static readonly Color WallColor = new(0.12f, 0.12f, 0.14f);
        private static readonly Color VacuumBorderColor = Color.white;
        private static readonly Color VacuumInnerColor = new(0.18f, 0.18f, 0.20f);
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

            if (_overlayShapeIndex == null || _overlayShapeIndex.GetLength(0) != w || _overlayShapeIndex.GetLength(1) != h)
            {
                _overlayShapeIndex = new int[w, h];
                _overlayConflict = new bool[w, h];
            }
            
            for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                _overlayShapeIndex[x, y] = -1; 
                _overlayConflict[x, y] = false;
            }

            _issuesBuilder.Clear();

            if (shapes != null)
            {
                for (int i = 0; i < shapes.Count; i++)
                {
                    var p = shapes[i];
                    if (p?.definition == null)
                        continue;

                    p.definition.CollectCells(p.anchor, p.rotation, CellBuffer);

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

                        if (_overlayShapeIndex[cell.x, cell.y] != -1)
                        {
                            _overlayConflict[cell.x, cell.y] = true;
                            _issuesBuilder.AppendLine($"Overlap at ({cell.x},{cell.y}).");
                        }

                        _overlayShapeIndex[cell.x, cell.y] = i; 
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

            if (self != null)
            {
                var e = Event.current;
                int shapeIndex = self._overlayShapeIndex[x, y];

                if (rect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDown && e.button == 0 && shapeIndex != -1)
                    {
                        _draggingShapeIndex = shapeIndex;
                        var shape = self.shapes[shapeIndex];
                        _dragOffset = new Vector2Int(x - shape.anchor.x, y - shape.anchor.y);
                        e.Use();
                    }
                    else if (e.type == EventType.MouseDrag && e.button == 0 && _draggingShapeIndex != -1)
                    {
                        var shape = self.shapes[_draggingShapeIndex];
                        var newAnchor = new Vector2Int(x - _dragOffset.x, y - _dragOffset.y);
                        
                        if (shape.anchor != newAnchor)
                        {
                            shape.anchor = newAnchor;
                            GUI.changed = true;
                        }
                        e.Use();
                    }
                    else if (e.isKey && e.type == EventType.KeyDown)
                    {
                        // EĞER SHIFT'E BASILIYSA -> RENK BOYAMA (Sadece VacuumBox için)
                        if (e.shift && value == CellType.VacuumBox)
                        {
                            ShapeColor? paintColor = e.keyCode switch
                            {
                                KeyCode.Keypad0 or KeyCode.Alpha0 => ShapeColor.None,
                                KeyCode.Keypad1 or KeyCode.Alpha1 => ShapeColor.Red,
                                KeyCode.Keypad2 or KeyCode.Alpha2 => ShapeColor.Blue,
                                KeyCode.Keypad3 or KeyCode.Alpha3 => ShapeColor.Green,
                                KeyCode.Keypad4 or KeyCode.Alpha4 => ShapeColor.Yellow,
                                KeyCode.Keypad5 or KeyCode.Alpha5 => ShapeColor.Orange,
                                KeyCode.Keypad6 or KeyCode.Alpha6 => ShapeColor.Purple,
                                _ => null
                            };

                            if (paintColor.HasValue && self.cellColors[x, y] != paintColor.Value)
                            {
                                self.cellColors[x, y] = paintColor.Value;
                                GUI.changed = true;
                                e.Use();
                            }
                        }
                        // SHIFT'E BASILI DEĞİLSE -> ZEMİN TİPİ BOYAMA
                        else if (!e.shift) 
                        {
                            CellType? paintType = e.keyCode switch
                            {
                                KeyCode.Keypad0 or KeyCode.Alpha0 => CellType.Empty,
                                KeyCode.Keypad1 or KeyCode.Alpha1 => CellType.Fillable,
                                KeyCode.Keypad2 or KeyCode.Alpha2 => CellType.Block,
                                KeyCode.Keypad3 or KeyCode.Alpha3 => CellType.Wall,
                                KeyCode.Keypad4 or KeyCode.Alpha4 => CellType.VacuumBox,
                                _ => null
                            };

                            if (paintType.HasValue && value != paintType.Value)
                            {
                                value = paintType.Value;
                                
                                // Eğer VacuumBox'tan başka bir şeye geçiş yapılıyorsa iç rengini sıfırla ki çöp data kalmasın
                                if (value != CellType.VacuumBox) 
                                    self.cellColors[x, y] = ShapeColor.None;

                                GUI.changed = true;
                                e.Use();
                            }
                        }
                    }
                }

                if (e.rawType == EventType.MouseUp && e.button == 0)
                {
                    if (_draggingShapeIndex != -1)
                    {
                        _draggingShapeIndex = -1;
                        GUI.changed = true;
                    }
                }
            }

            // Alt Zemini (Dış Çerçeveyi) Çiz
            EditorGUI.DrawRect(Inset(rect, BasePadding), BoardColor(value));

            // VacuumBox Özel Çizimi: Dış çerçeve beyaz kalır, içi seçilen renge boyanır
            if (value == CellType.VacuumBox)
            {
                var vColor = self != null ? self.cellColors[x, y] : ShapeColor.None;
                Color innerColor = vColor == ShapeColor.None ? VacuumInnerColor : ShapeToColor(vColor);
                
                EditorGUI.DrawRect(Inset(rect, BasePadding + 2.5f), innerColor);
                
                var style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    fontSize = 9,
                    normal = new GUIStyleState { textColor = Color.white }
                };
                GUI.Label(rect, "VAC", style); 
            }

            // Üst Katmanı (Şekil) Çiz
            if (self != null && x < self.Width && y < self.Height)
            {
                if (self._overlayConflict[x, y])
                    EditorGUI.DrawRect(Inset(rect, OverlayPadding), ConflictColor);
                else if (self._overlayShapeIndex[x, y] != -1)
                {
                    var color = self.shapes[self._overlayShapeIndex[x, y]].color;
                    EditorGUI.DrawRect(Inset(rect, OverlayPadding), ShapeToColor(color));
                }
            }

            return value;
        }

        private static Color BoardColor(CellType type) => type switch
        {
            CellType.Fillable => FillableColor,
            CellType.Block => BlockColor,
            CellType.Wall => WallColor,
            CellType.VacuumBox => VacuumBorderColor,
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