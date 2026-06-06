using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Game.Core.Shapes
{
    public class PolyominoShape : Shape
    {
        protected override void Setup(Material material, float cellSize)
        {
            var offsets = Cells;
            units = new Unit[offsets.Count];

            for (int i = 0; i < offsets.Count; i++)
            {
                var unit = pool.Get(unitPrefab, transform);
                unit.transform.localPosition = new Vector3(offsets[i].x * cellSize, 0f, -offsets[i].y * cellSize);
                unit.SetMaterial(material);
                units[i] = unit;
            }
        }
        
        public override void AnimateLift()
        {
            transform.DOKill();
            transform.DOMoveY(data.yOffset + data.liftHeight, data.liftDuration);
        }

        public override void AnimateDrop(Vector3 targetWorldPos)
        {
            targetWorldPos.y = data.yOffset;
            transform.DOKill();
            transform.DOMove(targetWorldPos, data.dropDuration);
        }

        public override void AnimateSwallow(Vector3 targetWorldPos)
        {
            transform.DOKill();

            Vector3 moveDelta = targetWorldPos - transform.position;
            bool isHorizontal = Mathf.Abs(moveDelta.x) > Mathf.Abs(moveDelta.z);
            
            Vector3 finalTarget = targetWorldPos;

            if (isHorizontal)
                transform.DOScaleX(0f, data.swallowDuration).SetEase(Ease.InQuad);
            else
            {
                if (moveDelta.z < 0f)
                    finalTarget.z -= data.swallowDepth;

                transform.DOScaleZ(0f, data.swallowDuration).SetEase(Ease.InQuad);
            }

            transform.DOMove(finalTarget, data.swallowDuration).SetEase(Ease.InQuad).OnComplete(() =>
            {
                transform.localScale = Vector3.one;
                pool.Release(this);
            });
        }
    }
}
