using System.Collections.Generic;
using _Game.Core.Pool;
using _Game.Data;
using _Game.Enums;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Game.Core.Shapes
{
    public abstract class Shape : MonoBehaviour, IPoolable
    {
        [SerializeField] protected ShapeData data;
        [SerializeField] protected Unit unitPrefab;

        protected Unit[] units;
        protected IPoolService pool;

        private readonly List<Vector2Int> _cells = new();

        public ShapeDefinition Definition { get; private set; }
        public ProductionColor Color { get; private set; }
        public Vector2Int Anchor { get; private set; }
        public ShapeRotation Rotation { get; private set; }
        public Vector2Int Size { get; private set; }

        public IReadOnlyList<Vector2Int> Cells => _cells;

        public float SwallowDuration => data.swallowDuration;
        public float DragSmoothTime => data.dragSmoothTime;

        public void Build(ShapeDefinition definition, ProductionColor color, Vector2Int anchor,
            ShapeRotation rotation, Material material, float cellSize, IPoolService poolService)
        {
            Definition = definition;
            Color = color;
            Anchor = anchor;
            Rotation = rotation;
            pool = poolService;

            definition.GetRotatedOffsets(rotation, _cells);
            Size = definition.GetSize(rotation);

            Setup(material, cellSize);
        }

        public void SetAnchor(Vector2Int anchor) => Anchor = anchor;

        public void CollectCells(Vector2Int anchor, List<Vector2Int> buffer)
        {
            buffer.Clear();
            for (int i = 0; i < _cells.Count; i++)
                buffer.Add(anchor + _cells[i]);
        }

        protected abstract void Setup(Material material, float cellSize);
        public abstract void AnimateLift();
        public abstract void AnimateDrop(Vector3 targetWorldPos);
        public abstract void AnimateSwallow(Vector3 targetWorldPos, bool horizontal);
        
        public virtual void OnSpawn() { }

        public virtual void OnDespawn()
        {
            transform.DOKill();

            if (units == null)
                return;

            for (int i = 0; i < units.Length; i++)
                pool.Release(units[i]);

            units = null;
        }
    }
}
