using _Game.Services;
using UnityEngine;

namespace _Game.Core.Cameras
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] private Camera cam;
        [SerializeField] private LayerMask interactionMask;

        public Camera Camera => cam;
        public LayerMask InteractionMask => interactionMask;
    }
}
