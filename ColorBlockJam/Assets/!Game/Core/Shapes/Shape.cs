using _Game.Core.Pool;
using _Game.Data;
using _Game.Enums;
using UnityEngine;

namespace _Game.Core.Shapes
{
    public abstract class Shape : MonoBehaviour, IPoolable
    {
        [SerializeField] protected Unit unitPrefab;

        protected Unit[] units;
        protected IPoolService pool;

        public ShapeDefinition Definition { get; private set; }
        public ShapeColor Color { get; private set; }
        public Vector2Int Anchor { get; private set; }

        public void Build(ShapeDefinition definition, ShapeColor color, Vector2Int anchor,
            Material material, float cellSize, IPoolService poolService)
        {
            Definition = definition;
            Color = color;
            Anchor = anchor;
            pool = poolService;

            Setup(material, cellSize);
        }

        public void SetAnchor(Vector2Int anchor) => Anchor = anchor;

        protected abstract void Setup(Material material, float cellSize);

        public virtual void OnSpawn() { }

        public virtual void OnDespawn()
        {
            if (units == null)
                return;

            for (int i = 0; i < units.Length; i++)
                pool.Release(units[i]);

            units = null;
        }
    }
}
