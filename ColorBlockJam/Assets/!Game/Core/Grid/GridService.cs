using System.Collections.Generic;
using _Game.Core.Interactable;
using _Game.Core.Pool;
using _Game.Core.Shapes;
using _Game.Data;
using _Game.Enums;
using UnityEngine;

namespace _Game.Core.Grid
{
    public class GridService : IGridService
    {
        private const string RootName = "[GridRoot]";
        private const float Half = 0.5f;

        private readonly GridData _data;
        private readonly IPoolService _pool;
        private readonly List<Vector2Int> _cellBuffer = new();
        private readonly List<Transform> _edgeWalls = new(); 
        private readonly List<InteractableBox> _mechanics = new();

        private LevelData _level;
        private Tile[,] _tiles;
        private Shape[,] _occupants;
        private Transform _root;
        private int _width;
        private int _height;

        public float CellSize => _data.cellSize;

        public GridService(GridData data, IPoolService pool)
        {
            _data = data;
            _pool = pool;
        }

        public void Build(LevelData level)
        {
            if (!_data || !level) return;

            Clear();

            _level = level;
            _width = level.Width;
            _height = level.Height;
            _tiles = new Tile[_width, _height];
            _occupants = new Shape[_width, _height];
            
            if (!_root)
                _root = new GameObject(RootName).transform;

            for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
            {
                var cellType = level.GetCell(x, y);
                
                if (cellType == CellType.Empty ||
                    cellType == CellType.Wall ||
                    cellType == CellType.VacuumBox || 
                    cellType == CellType.Block)
                    continue;

                var tile = _pool.Get(_data.tilePrefab, _root);
                tile.Setup(new Vector2Int(x, y), CoordToLocal(x, y));
                _tiles[x, y] = tile;
            }
        }

        public void Clear()
        {
            if (_tiles != null)
            {
                int w = _tiles.GetLength(0), h = _tiles.GetLength(1);
                for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (_tiles[x, y])
                        _pool.Release(_tiles[x, y]);
            }

            _tiles = null;
            _occupants = null;
            _level = null;
            _width = 0;
            _height = 0;
        }

        public bool IsInside(int x, int y) => x >= 0 && y >= 0 && x < _width && y < _height;

        public bool IsCellFree(int x, int y, Shape ignore)
        {
            if (!IsInside(x, y))
                return false;
            if (_level.GetCell(x, y) != CellType.Fillable)
                return false;

            var occupant = _occupants[x, y];
            return !occupant || occupant == ignore;
        }

        public void Occupy(Shape shape) => Mark(shape, shape);
        public void Free(Shape shape) => Mark(shape, null);

        private void Mark(Shape shape, Shape value)
        {
            if (!shape || shape.Definition == null)
                return;

            shape.CollectCells(shape.Anchor, _cellBuffer);
            for (int i = 0; i < _cellBuffer.Count; i++)
            {
                var c = _cellBuffer[i];
                if (!IsInside(c.x, c.y))
                    continue;
                if (value == null && _occupants[c.x, c.y] != shape)
                    continue; 
                _occupants[c.x, c.y] = value;
            }
        }

        public Vector3 CoordToWorld(int x, int y) => CellToWorld(new Vector2(x, y));

        public Vector3 CellToWorld(Vector2 cell)
        {
            var local = CoordToLocal(cell.x, cell.y);
            return _root ? _root.TransformPoint(local) : local;
        }

        public Vector2 WorldToCell(Vector3 world)
        {
            var local = _root ? _root.InverseTransformPoint(world) : world;
            var step = _data.cellSize;
            var fx = local.x / step;
            var fz = local.z / step;

            if (_data.centered)
            {
                fx += (_width - 1) * Half;
                fz += (_height - 1) * Half;
            }

            return new Vector2(fx, (_height - 1) - fz);
        }

        private Vector3 CoordToLocal(float x, float y)
        {
            var step = _data.cellSize;
            var px = x * step;
            var pz = (_height - 1 - y) * step;

            if (_data.centered)
            {
                px -= (_width - 1) * step * Half;
                pz -= (_height - 1) * step * Half;
            }

            return new Vector3(px, 0f, pz);
        }
    }
}