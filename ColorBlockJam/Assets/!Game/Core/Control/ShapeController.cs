using System.Collections.Generic;
using _Game.Core.Grid;
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
        private DragSettings _settings;
        private Shape _shape;
        private Vector2 _offset;
        private Vector2 _cell;
        private Vector2Int _origin;

        public void Init(IGridService grid, DragSettings settings)
        {
            _grid = grid;
            _settings = settings;

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
            if (_shape || !e.Shape)
                return;

            _shape = e.Shape;
            _origin = _shape.Anchor;
            _cell = _origin;

            var pos = _shape.transform.position;
            _offset = new Vector2(pos.x - e.WorldPoint.x, pos.z - e.WorldPoint.z);

            _shape.transform.DOKill();
            _shape.transform.DOMoveY(_settings.liftHeight, _settings.liftDuration);
        }

        private void OnDragged(ShapeDraggedEvent e)
        {
            if (!_shape)
                return;

            var desired = new Vector3(e.WorldPoint.x + _offset.x, 0f, e.WorldPoint.z + _offset.y);
            var want = ClampToBounds(_grid.WorldToCell(desired));

            _cell = Resolve(_cell, want);

            var world = _grid.CellToWorld(_cell);
            var pos = _shape.transform.position;
            _shape.transform.position = new Vector3(world.x, pos.y, world.z);
        }

        private void OnReleased(ShapeReleasedEvent e)
        {
            if (!_shape)
                return;

            var target = new Vector2Int(Mathf.RoundToInt(_cell.x), Mathf.RoundToInt(_cell.y));
            if (!FootprintFree(target))
                target = _origin;

            _grid.Free(_shape);
            _shape.SetAnchor(target);
            _grid.Occupy(_shape);

            _shape.transform.DOKill();
            _shape.transform.DOMove(_grid.CoordToWorld(target.x, target.y), _settings.dropDuration);

            _shape = null;
        }

        private Vector2 ClampToBounds(Vector2 cell)
        {
            var size = _shape.Definition.Size;
            float maxX = Mathf.Max(0, _grid.Width - size.x);
            float maxY = Mathf.Max(0, _grid.Height - size.y);
            return new Vector2(Mathf.Clamp(cell.x, 0f, maxX), Mathf.Clamp(cell.y, 0f, maxY));
        }

        private Vector2 Resolve(Vector2 current, Vector2 target)
        {
            float x = StepAxis(current, target.x, true);
            float y = StepAxis(new Vector2(x, current.y), target.y, false);
            return new Vector2(x, y);
        }

        private float StepAxis(Vector2 from, float target, bool xAxis)
        {
            float value = xAxis ? from.x : from.y;
            float dir = Mathf.Sign(target - value);
            if (value == target || Mathf.Approximately(dir, 0f))
                return value;

            while (Mathf.Abs(target - value) > 0.0001f)
            {
                float next = value + dir * Mathf.Min(SubStep, Mathf.Abs(target - value));
                var probe = xAxis ? new Vector2(next, from.y) : new Vector2(from.x, next);
                if (!FootprintFreeContinuous(probe))
                    break;
                value = next;
            }

            return value;
        }

        private bool FootprintFreeContinuous(Vector2 cell)
        {
            var offsets = _shape.Definition.Offsets;
            for (int i = 0; i < offsets.Count; i++)
            {
                float wx = cell.x + offsets[i].x;
                float wy = cell.y + offsets[i].y;
                int x0 = Mathf.FloorToInt(wx), x1 = Mathf.CeilToInt(wx);
                int y0 = Mathf.FloorToInt(wy), y1 = Mathf.CeilToInt(wy);

                if (!_grid.IsCellFree(x0, y0, _shape) || !_grid.IsCellFree(x1, y0, _shape) ||
                    !_grid.IsCellFree(x0, y1, _shape) || !_grid.IsCellFree(x1, y1, _shape))
                    return false;
            }

            return true;
        }

        private bool FootprintFree(Vector2Int anchor)
        {
            _shape.Definition.CollectCells(anchor, _buffer);
            for (int i = 0; i < _buffer.Count; i++)
                if (!_grid.IsCellFree(_buffer[i].x, _buffer[i].y, _shape))
                    return false;

            return true;
        }
    }
}
