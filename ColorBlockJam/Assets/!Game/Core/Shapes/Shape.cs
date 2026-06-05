using _Game.Data;
using _Game.Enums;
using UnityEngine;

namespace _Game.Core.Shapes
{
    public abstract class Shape : MonoBehaviour
    {
        [SerializeField] protected Unit unitPrefab;

        protected Unit[] units;

        public ShapeDefinition Definition { get; private set; }
        public ShapeColor Color { get; private set; }
        public Vector2Int Anchor { get; private set; }

        public void Build(ShapeDefinition definition, ShapeColor color, Vector2Int anchor, Material material, float cellSize)
        {
            Definition = definition;
            Color = color;
            Anchor = anchor;

            Setup(material, cellSize);
        }
        
        protected abstract void Setup(Material material, float cellSize);
    }
}
