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

        private readonly List<Shape> _shapes = new();
        private Transform _root;

        public ShapeFactory(Shape shapePrefab, ColorPalette palette, IGridService grid, IPoolService pool)
        {
            _shapePrefab = shapePrefab;
            _palette = palette;
            _grid = grid;
            _pool = pool;
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

            shape.Build(placement.definition, placement.color, placement.anchor, material, _grid.CellSize, _pool);
            shape.transform.position = _grid.CoordToWorld(placement.anchor.x, placement.anchor.y);
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
