using System.Collections.Generic;
using _Game.Core.Grid;
using _Game.Core.Match;
using _Game.Core.Shapes;
using _Game.Data;
using _Game.Events;
using DG.Tweening;
using UnityEngine;

namespace _Game.Core.Control
{
    public class ShapeController : MonoBehaviour, IShapeController
    {
        private const float SubStep = 0.1f;

        private readonly List<Vector2Int> _buffer = new();
        private IGridService _grid;
        private IMatchController _matchController; 
        private Shape _shape;
        private Vector2 _offset;
        private Vector2 _cell;
        private Vector2Int _origin;

        public void Init(IGridService grid, IMatchController matchController)
        {
            _grid = grid;
            _matchController = matchController;

            EventBus.Subscribe<ShapeGrabbedEvent>(OnGrabbed);
            EventBus.Subscribe<ShapeDraggedEvent>(OnDragged);
            EventBus.Subscribe<ShapeReleasedEvent>(OnReleased);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<ShapeGrabbedEvent>(OnGrabbed);
            EventBus.Unsubscribe<ShapeDraggedEvent>(OnDragged);
            EventBus.Unsubscribe<ShapeReleasedEvent>(OnReleased);
        }

        private void OnGrabbed(ShapeGrabbedEvent e)
        {
            if (_shape || !e.Shape) return;

            _shape = e.Shape;
            _origin = _shape.Anchor;
            _cell = _origin;

            var pos = _shape.transform.position;
            _offset = new Vector2(pos.x - e.WorldPoint.x, pos.z - e.WorldPoint.z);

            _shape.AnimateLift();
        }

        private void OnDragged(ShapeDraggedEvent e)
        {
            if (!_shape) return;
            _grid.Free(_shape);

            var desired = new Vector3(e.WorldPoint.x + _offset.x, 0f, e.WorldPoint.z + _offset.y);
            var wantCell = _grid.WorldToCell(desired);

            _cell = Resolve(_cell, wantCell);

            if (_shape) _grid.Occupy(_shape); 

            if (!_shape) return;

            var world = _grid.CellToWorld(_cell);
            var pos = _shape.transform.position;
            _shape.transform.position = new Vector3(world.x, pos.y, world.z);
        }

        private void OnReleased(ShapeReleasedEvent e)
        {
            if (!_shape) return;

            var target = new Vector2Int(Mathf.RoundToInt(_cell.x), Mathf.RoundToInt(_cell.y));
            if (!FootprintFree(target))
                target = _origin;

            _grid.Free(_shape);
            _shape.SetAnchor(target);
            _grid.Occupy(_shape);

            var world = _grid.CoordToWorld(target.x, target.y);
            _shape.AnimateDrop(world);
            _shape = null;
        }

        private Vector2 Resolve(Vector2 current, Vector2 target)
        {
            var x = StepAxis(current, target.x, true);
            if (!_shape) return new Vector2(x, current.y); 

            var y = StepAxis(new Vector2(x, current.y), target.y, false);
            return new Vector2(x, y);
        }

        private float StepAxis(Vector2 from, float target, bool xAxis)
        {
            var value = xAxis ? from.x : from.y;
            var dir = Mathf.Sign(target - value);
            if (Mathf.Approximately(value, target) || Mathf.Approximately(dir, 0f))
                return value;

            while (Mathf.Abs(target - value) > 0.0001f)
            {
                float next = value + dir * Mathf.Min(SubStep, Mathf.Abs(target - value));
                var probe = xAxis ? new Vector2(next, from.y) : new Vector2(from.x, next);

                if (!FootprintFreeContinuous(probe))
                {
                    var currentPos = new Vector2(xAxis ? value : from.x, xAxis ? from.y : value);
                    
                    if (_matchController.TrySwallow(_shape, currentPos, probe))
                        _shape = null; 
                    
                    break; 
                }
                value = next;
            }

            return value;
        }

        private bool FootprintFreeContinuous(Vector2 cell)
        {
            var offsets = _shape.Cells;
            const float shrink = 0.1f; 

            for (int i = 0; i < offsets.Count; i++)
            {
                var wx = cell.x + offsets[i].x;
                var wy = cell.y + offsets[i].y;
                
                var x0 = Mathf.FloorToInt(wx + shrink);
                var x1 = Mathf.FloorToInt(wx + 1f - shrink);
                var y0 = Mathf.FloorToInt(wy + shrink);
                var y1 = Mathf.FloorToInt(wy + 1f - shrink);

                if (!_matchController.IsDraggable(_shape, x0, y0) || !_matchController.IsDraggable(_shape, x1, y0) ||
                    !_matchController.IsDraggable(_shape, x0, y1) || !_matchController.IsDraggable(_shape, x1, y1))
                    return false;
            }

            return true;
        }

        private bool FootprintFree(Vector2Int anchor)
        {
            _shape.CollectCells(anchor, _buffer);
            for (int i = 0; i < _buffer.Count; i++)
                if (!_grid.IsCellFree(_buffer[i].x, _buffer[i].y, _shape))
                    return false;

            return true;
        }
    }
}