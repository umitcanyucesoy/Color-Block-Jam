using _Game.Services;
using UnityEngine;

namespace _Game.Core.Pool
{
    public interface IPoolService : IService
    {
        T Get<T>(T prefab, Transform parent = null) where T : Component;
        void Release(Component instance);
        void Clear();
    }
}
