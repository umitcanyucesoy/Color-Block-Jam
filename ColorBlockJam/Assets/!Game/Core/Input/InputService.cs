using System.Threading;
using _Game.Core.Cameras;
using _Game.Core.Shapes;
using _Game.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Input
{
    public class InputService : IInputService
    {
        private const float RayDistance = 100f;

        private readonly ICameraController _cameraController;
        private readonly Plane _groundPlane = new(Vector3.up, Vector3.zero);
        private readonly CancellationTokenSource _cts = new();
        private bool _enabled = true;

        public InputService(ICameraController cameraController)
        {
            _cameraController = cameraController;
            DragLoop().Forget();
        }

        public void SetEnabled(bool enabled) => _enabled = enabled;

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        private async UniTaskVoid DragLoop()
        {
            var token = _cts.Token;

            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitUntil(
                    () => _enabled && UnityEngine.Input.GetMouseButtonDown(0),
                    cancellationToken: token);

                if (!TryGrab(out var shape, out var grabPoint))
                    continue;

                EventBus.Publish(new ShapeGrabbedEvent { Shape = shape, WorldPoint = grabPoint });

                while (UnityEngine.Input.GetMouseButton(0))
                {
                    if (TryPlanePoint(out var point))
                        EventBus.Publish(new ShapeDraggedEvent { WorldPoint = point });

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                EventBus.Publish(new ShapeReleasedEvent());
            }
        }

        private bool TryGrab(out Shape shape, out Vector3 worldPoint)
        {
            shape = null;
            worldPoint = default;

            var camera = _cameraController.Camera;
            if (!camera)
                return false;

            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, RayDistance, _cameraController.InteractionMask))
                return false;

            shape = hit.collider.GetComponentInParent<Shape>();
            if (!shape)
                return false;

            worldPoint = _groundPlane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : hit.point;
            return true;
        }

        private bool TryPlanePoint(out Vector3 point)
        {
            point = default;

            var camera = _cameraController.Camera;
            if (!camera)
                return false;

            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (!_groundPlane.Raycast(ray, out var enter))
                return false;

            point = ray.GetPoint(enter);
            return true;
        }
    }
}
