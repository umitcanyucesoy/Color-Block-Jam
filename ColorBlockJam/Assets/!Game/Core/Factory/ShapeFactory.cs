using System.Collections.Generic;
using _Game.Core.Grid;
using _Game.Core.Pool;
using _Game.Core.Shapes;
using _Game.Data;
using UnityEngine;

namespace _Game.Core.Factory
{
    public class ShapeFactory : IShapeFactory
    {
        private const string RootName = "[ShapeRoot]";

        private readonly Shape _shapePrefab;
        private readonly ColorPalette _palette;
        private readonly IGridService _grid;
        private readonly IPoolService _pool;
        private readonly ShapeData _data;

        private readonly List<Shape> _shapes = new();
        private Transform _root;

        public int ActiveCount => _shapes.Count;

        public ShapeFactory(Shape shapePrefab, ColorPalette palette, IGridService grid, IPoolService pool, ShapeData data)
        {
            _shapePrefab = shapePrefab;
            _palette = palette;
            _grid = grid;
            _pool = pool;
            _data = data;
        }

        public void BuildLevel(LevelData level)
        {
            if (_grid == null || _pool == null || !level || !_shapePrefab || !_palette)
            {
                Debug.LogError("[ShapeFactory] Missing dependency.");
                return;
            }

            Clear();
            EnsureRoot();

            var shapes = level.Shapes;
            for (int i = 0; i < shapes.Count; i++)
                Create(shapes[i]);
        }

        public void Clear()
        {
            for (int i = 0; i < _shapes.Count; i++)
                if (_shapes[i])
                    _pool.Release(_shapes[i]);

            _shapes.Clear();
        }

        private Shape Create(ShapePlacement placement)
        {
            if (!placement?.definition)
                return null;

            var material = _palette.GetMaterial(placement.color);
            var shape = _pool.Get(_shapePrefab, _root);

            shape.Build(placement.definition, placement.color, placement.anchor, placement.rotation, material, _grid.CellSize, _pool);
            var world = _grid.CoordToWorld(placement.anchor.x, placement.anchor.y);
            world.y = _data.yOffset;
            shape.transform.position = world;
            _grid.Occupy(shape);

            _shapes.Add(shape);
            return shape;
        }

        private void EnsureRoot()
        {
            if (!_root)
                _root = new GameObject(RootName).transform;
        }
    }
}
