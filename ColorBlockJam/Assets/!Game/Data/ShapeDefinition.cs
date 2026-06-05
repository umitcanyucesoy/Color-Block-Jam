using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "ShapeDefinition", menuName = "ColorBlockJam/Shape Definition", order = 2)]
    public partial class ShapeDefinition : SerializedScriptableObject
    {
        [HorizontalGroup("Split", Width = 120)]
        [VerticalGroup("Split/Left"), LabelWidth(50), MinValue(1), SerializeField]
        private int width = 3;
        [VerticalGroup("Split/Left"), LabelWidth(50), MinValue(1), SerializeField]
        private int height = 3;
        
        [VerticalGroup("Split/Right"), HideLabel]
        [TableMatrix(SquareCells = true, DrawElementMethod = "DrawCell", RowHeight = 5)]
        [OnValueChanged(nameof(Invalidate), IncludeChildren = true)]
        [SerializeField]
        private bool[,] grid;

        [NonSerialized] private List<Vector2Int> _offsets;

        public IReadOnlyList<Vector2Int> Offsets { get { EnsureBaked(); return _offsets; } }
        public int CellCount { get { EnsureBaked(); return _offsets.Count; } }

        public Vector2Int Size
        {
            get
            {
                EnsureBaked();
                if (_offsets.Count == 0)
                    return Vector2Int.zero;

                int maxX = 0, maxY = 0;
                for (int i = 0; i < _offsets.Count; i++)
                {
                    if (_offsets[i].x > maxX) maxX = _offsets[i].x;
                    if (_offsets[i].y > maxY) maxY = _offsets[i].y;
                }

                return new Vector2Int(maxX + 1, maxY + 1);
            }
        }

        public void CollectCells(Vector2Int anchor, List<Vector2Int> buffer)
        {
            EnsureBaked();
            buffer.Clear();
            for (int i = 0; i < _offsets.Count; i++)
                buffer.Add(anchor + _offsets[i]);
        }

        private void EnsureBaked()
        {
            if (_offsets == null)
                RebuildOffsets();
        }

        private void Invalidate() => _offsets = null;

        private void RebuildOffsets()
        {
            _offsets ??= new List<Vector2Int>();
            _offsets.Clear();
            if (grid == null)
                return;

            int w = grid.GetLength(0), h = grid.GetLength(1);
            int minX = int.MaxValue, minY = int.MaxValue;
            bool any = false;

            for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                if (grid[x, y])
                {
                    any = true;
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                }

            if (!any)
                return;

            for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                if (grid[x, y])
                    _offsets.Add(new Vector2Int(x - minX, y - minY));
        }

        [Button(ButtonSizes.Medium), PropertyOrder(10), PropertySpace(SpaceBefore = 8)]
        private void ResizeGrid()
        {
            var resized = new bool[width, height];

            if (grid != null)
            {
                int copyX = Mathf.Min(width, grid.GetLength(0));
                int copyY = Mathf.Min(height, grid.GetLength(1));

                for (int x = 0; x < copyX; x++)
                for (int y = 0; y < copyY; y++)
                    resized[x, y] = grid[x, y];
            }

            grid = resized;
            Invalidate();
        }

        [OnInspectorInit]
        private void EnsureGrid()
        {
            grid ??= new bool[width, height];
        }
    }
}
