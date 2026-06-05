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
        private readonly LevelData _level;
        private Tile[,] _tiles;
        private Transform _root;
        private int _width;
        private int _height;

        public int Width => _width;
        public int Height => _height;

        public GridService(GridData data, LevelData level)
        {
            _data = data;
            _level = level;
        }

        public void Build()
        {
            if (!_data || !_level)
            {
                Debug.LogError("[GridService] Missing GridConfig or LevelData.");
                return;
            }

            if (_root)
                Clear();

            _width = _level.Width;
            _height = _level.Height;
            _tiles = new Tile[_width, _height];
            _root = new GameObject(RootName).transform;

            for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
            {
                if (_level.GetCell(x, y) == CellType.Empty)
                    continue;

                var tile = Object.Instantiate(_data.tilePrefab, _root);
                tile.Setup(new Vector2Int(x, y), CoordToLocal(x, y));
                _tiles[x, y] = tile;
            }
        }

        public void Clear()
        {
            if (_root) Object.Destroy(_root.gameObject);

            _tiles = null;
            _root = null;
            _width = 0;
            _height = 0;
        }

        public bool IsInside(int x, int y) => x >= 0 && y >= 0 && x < _width && y < _height;
        public bool IsInside(Vector2Int coordinate) => IsInside(coordinate.x, coordinate.y);

        public Tile GetTile(int x, int y) => IsInside(x, y) ? _tiles[x, y] : null;

        public bool TryGetTile(int x, int y, out Tile tile)
        {
            if (IsInside(x, y))
            {
                tile = _tiles[x, y];
                return tile;
            }

            tile = null;
            return false;
        }

        public Vector3 CoordToWorld(int x, int y)
        {
            var local = CoordToLocal(x, y);
            return _root ? _root.TransformPoint(local) : local;
        }

        private Vector3 CoordToLocal(int x, int y)
        {
            var step = _data.cellSize;
            var px = x * step;
            var pz = y * step;

            if (_data.centered)
            {
                px -= (_width - 1) * step * Half;
                pz -= (_height - 1) * step * Half;
            }

            return new Vector3(px, 0f, pz);
        }
    }
}
