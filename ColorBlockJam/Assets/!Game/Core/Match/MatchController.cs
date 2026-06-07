using System.Collections.Generic;
using _Game.Core.Grid;
using _Game.Core.Interactable;
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
        private IVacuumBoxController _vacuums;

        public void Init(IGridService grid, ILevelController levelController, IPoolService pool, IVacuumBoxController vacuums)
        {
            _grid = grid;
            _levelController = levelController;
            _pool = pool;
            _vacuums = vacuums;
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
                var wx = probePos.x + offsets[i].x;
                var wy = probePos.y + offsets[i].y;
                
                var x0 = Mathf.FloorToInt(wx + shrink);
                var x1 = Mathf.FloorToInt(wx + 1f - shrink);
                var y0 = Mathf.FloorToInt(wy + shrink);
                var y1 = Mathf.FloorToInt(wy + 1f - shrink);

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
                var horizontal = !Mathf.Approximately(probePos.x, currentPos.x);

                var vacuumCell = hitVacuumCell.Value;
                var unitOffset = hitUnitOffset.Value;
                
                var nearest = horizontal
                    ? new Vector2Int(vacuumCell.x, Mathf.RoundToInt(currentPos.y + unitOffset.y))
                    : new Vector2Int(Mathf.RoundToInt(currentPos.x + unitOffset.x), vacuumCell.y);

                if (IsMatchingVacuum(shape, nearest.x, nearest.y))
                    vacuumCell = nearest;

                var snapAnchor = new Vector2Int(Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.y));
                var targetAnchor = new Vector2Int(vacuumCell.x - unitOffset.x, vacuumCell.y - unitOffset.y);

                ExecuteSwallow(shape, snapAnchor, targetAnchor, vacuumCell, horizontal);
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

        private void ExecuteSwallow(Shape shape, Vector2Int snapAnchor, Vector2Int targetAnchor, Vector2Int vacuumCell, bool horizontal)
        {
            _grid.Free(shape);
            shape.transform.DOKill();

            float y = shape.transform.position.y;

            var snapWorld = _grid.CoordToWorld(snapAnchor.x, snapAnchor.y);
            var anchorWorld = _grid.CoordToWorld(targetAnchor.x, targetAnchor.y);
            var vacuumWorld = _grid.CoordToWorld(vacuumCell.x, vacuumCell.y);

            var startPos = anchorWorld;
            var finalTarget = anchorWorld;
            if (horizontal)
            {
                startPos.x = snapWorld.x;
                finalTarget.x = vacuumWorld.x;
            }
            else
            {
                startPos.z = snapWorld.z;
                finalTarget.z = vacuumWorld.z;
            }
            startPos.y = y;
            finalTarget.y = y;

            shape.transform.position = startPos;
            _vacuums.PlaySwallow(vacuumCell);
            shape.AnimateSwallow(finalTarget, horizontal);
        }
    }
}