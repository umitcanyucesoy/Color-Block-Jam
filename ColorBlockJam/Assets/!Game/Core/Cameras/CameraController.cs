using _Game.Services;
using UnityEngine;

namespace _Game.Core.Cameras
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] private Camera cam;
        [SerializeField] private LayerMask interactionMask;

        [Header("Width-based FOV")]
        [SerializeField] private int baseWidth = 10;
        [SerializeField] private float fovPerWidthUnit = 6f;

        private float _baseFov;

        public Camera Camera => cam;
        public LayerMask InteractionMask => interactionMask;

        private void Awake() => _baseFov = cam.fieldOfView;

        public void ApplyWidth(int width)
        {
            if (!cam) return;

            var extra = Mathf.Max(0, width - baseWidth) * fovPerWidthUnit;
            cam.fieldOfView = _baseFov + extra;
        }
    }
}
