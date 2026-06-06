using System.Collections.Generic;
using _Game.Core.Grid;
using _Game.Core.Level;
using _Game.Core.Pool;
using _Game.Core.Shapes;
using _Game.Enums;
using DG.Tweening;
using UnityEngine;

namespace _Game.Core.Match
{
    public class MatchController : MonoBehaviour, IMatchController
    {
        private IGridService _grid;
        private ILevelController _levelController;
        private IPoolService _pool;

        public void Init(IGridService grid, ILevelController levelController, IPoolService pool)
        {
            _grid = grid;
            _levelController = levelController;
            _pool = pool;
        }

        public bool IsDraggable(Shape shape, int x, int y)
        {
            if (!_grid.IsInside(x, y)) return false;
            
            var cellType = _levelController.Current.GetCell(x, y);

            if (cellType == CellType.Fillable)
                return _grid.IsCellFree(x, y, shape);

            return false;
        }

        public bool TrySwallow(Shape shape, Vector2 currentPos, Vector2 probePos)
        {
            var level = _levelController.Current;
            if (level == null) return false;

            const float shrink = 0.1f;
            var offsets = shape.Cells;

            Vector2Int? hitVacuumCell = null;
            Vector2Int? hitUnitOffset = null;

            for (int i = 0; i < offsets.Count; i++)
            {
                float wx = probePos.x + offsets[i].x;
                float wy = probePos.y + offsets[i].y;
                
                int x0 = Mathf.FloorToInt(wx + shrink);
                int x1 = Mathf.FloorToInt(wx + 1f - shrink);
                int y0 = Mathf.FloorToInt(wy + shrink);
                int y1 = Mathf.FloorToInt(wy + 1f - shrink);

                if (!IsWalkableOrMatchingVacuum(shape, x0, y0)) return false;
                if (!IsWalkableOrMatchingVacuum(shape, x1, y0)) return false;
                if (!IsWalkableOrMatchingVacuum(shape, x0, y1)) return false;
                if (!IsWalkableOrMatchingVacuum(shape, x1, y1)) return false;

                if (!hitVacuumCell.HasValue)
                {
                    if (IsMatchingVacuum(shape, x0, y0)) { hitVacuumCell = new Vector2Int(x0, y0); hitUnitOffset = offsets[i]; }
                    else if (IsMatchingVacuum(shape, x1, y0)) { hitVacuumCell = new Vector2Int(x1, y0); hitUnitOffset = offsets[i]; }
                    else if (IsMatchingVacuum(shape, x0, y1)) { hitVacuumCell = new Vector2Int(x0, y1); hitUnitOffset = offsets[i]; }
                    else if (IsMatchingVacuum(shape, x1, y1)) { hitVacuumCell = new Vector2Int(x1, y1); hitUnitOffset = offsets[i]; }
                }
            }

            if (hitVacuumCell.HasValue)
            {
                Vector2Int snapAnchor = new Vector2Int(Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.y));
                Vector2Int targetAnchor = new Vector2Int(hitVacuumCell.Value.x - hitUnitOffset.Value.x, hitVacuumCell.Value.y - hitUnitOffset.Value.y);

                ExecuteSwallow(shape, snapAnchor, targetAnchor);
                return true;
            }

            return false;
        }

        private bool IsWalkableOrMatchingVacuum(Shape shape, int x, int y)
        {
            if (!_grid.IsInside(x, y)) return false;
            var cellType = _levelController.Current.GetCell(x, y);

            if (cellType == CellType.Fillable)
                return _grid.IsCellFree(x, y, shape);

            if (cellType == CellType.VacuumBox && _levelController.Current.GetCellColor(x, y) == shape.Color)
                return true; 

            return false; 
        }

        private bool IsMatchingVacuum(Shape shape, int x, int y)
        {
            if (!_grid.IsInside(x, y)) return false;
            return _levelController.Current.GetCell(x, y) == CellType.VacuumBox && _levelController.Current.GetCellColor(x, y) == shape.Color;
        }

        private void ExecuteSwallow(Shape shape, Vector2Int snapAnchor, Vector2Int targetAnchor)
        {
            _grid.Free(shape);
            shape.transform.DOKill();

            Vector3 snapWorldPos = _grid.CoordToWorld(snapAnchor.x, snapAnchor.y);
            snapWorldPos.y = shape.transform.position.y;
            shape.transform.position = snapWorldPos;

            Vector3 targetWorldPos = _grid.CoordToWorld(targetAnchor.x, targetAnchor.y);
            targetWorldPos.y = snapWorldPos.y;

            var seq = DOTween.Sequence();
            
            seq.Append(shape.transform.DOMove(targetWorldPos, 0.45f).SetEase(Ease.InOutSine));
            seq.Join(shape.transform.DOScale(Vector3.zero, 0.45f).SetEase(Ease.InBack)); 
            
            seq.OnComplete(() => {
                shape.transform.localScale = Vector3.one; 
                _pool.Release(shape);
            });
        }
    }
}