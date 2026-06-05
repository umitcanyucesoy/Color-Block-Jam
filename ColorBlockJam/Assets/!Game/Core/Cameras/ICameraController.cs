using _Game.Services;
using UnityEngine;

namespace _Game.Core.Cameras
{
    public interface ICameraController : IService
    {
        Camera Camera { get; }
        LayerMask InteractionMask { get; }
    }
}
