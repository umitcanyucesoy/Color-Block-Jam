using _Game.Core.Grid;
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

        private Transform _root;

        public ShapeFactory(Shape shapePrefab, ColorPalette palette, IGridService grid)
        {
            _shapePrefab = shapePrefab;
            _palette = palette;
            _grid = grid;
        }

        public void BuildLevel(LevelData level)
        {
            if (_grid == null || !level || !_shapePrefab || !_palette)
            {
                Debug.LogError("[ShapeFactory] Missing dependency.");
                return;
            }

            if (_root)
                Clear();

            _root = new GameObject(RootName).transform;

            var shapes = level.Shapes;
            for (int i = 0; i < shapes.Count; i++)
                Create(shapes[i]);
        }

        public void Clear()
        {
            if (_root) Object.Destroy(_root.gameObject);
            _root = null;
        }

        private Shape Create(ShapePlacement placement)
        {
            if (!placement?.definition)
                return null;

            var material = _palette.GetMaterial(placement.color);
            var shape = Object.Instantiate(_shapePrefab, _root);

            shape.Build(placement.definition, placement.color, placement.anchor, material, _grid.CellSize);
            shape.transform.position = _grid.CoordToWorld(placement.anchor.x, placement.anchor.y);

            return shape;
        }
    }
}
