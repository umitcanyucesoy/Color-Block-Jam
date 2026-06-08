using System.Collections.Generic;
using _Game.Data;
using UnityEngine;

namespace _Game.Core.Pool
{
    public class PoolService : IPoolService
    {
        private const string RootName = "[PoolRoot]";

        private class Pooled
        {
            public GameObject Prefab;
            public Component Component;
            public bool InPool;
        }

        private readonly Dictionary<GameObject, Stack<GameObject>> _available = new();
        private readonly Dictionary<GameObject, Pooled> _pooled = new();
        private Transform _root;

        public PoolService(PoolData data)
        {
            _root = new GameObject(RootName).transform;

            if (data)
                Prewarm(data);
        }

        public T Get<T>(T prefab, Transform parent = null) where T : Component
        {
            var key = prefab.gameObject;
            var stack = GetStack(key);

            Component component;

            if (stack.Count > 0)
            {
                var go = stack.Pop();
                var pooled = _pooled[go];
                pooled.Component ??= go.GetComponent<T>();
                pooled.InPool = false;
                component = pooled.Component;
            }
            else
            {
                component = Object.Instantiate(prefab);
                _pooled[component.gameObject] = new Pooled { Prefab = key, Component = component, InPool = false };
            }

            component.transform.SetParent(parent, false);
            component.gameObject.SetActive(true);

            if (component is IPoolable poolable)
                poolable.OnSpawn();

            return (T)component;
        }

        public void Release(Component instance)
        {
            if (!instance)
                return;

            var go = instance.gameObject;

            if (!_pooled.TryGetValue(go, out var pooled))
            {
                Debug.LogWarning($"[PoolService] '{go.name}' was not pooled. Destroying.");
                Object.Destroy(go);
                return;
            }

            if (pooled.InPool) 
                return;
            pooled.InPool = true;

            if (instance is IPoolable poolable)
                poolable.OnDespawn();

            go.SetActive(false);
            go.transform.SetParent(_root, false);
            GetStack(pooled.Prefab).Push(go);
        }

        public void Clear()
        {
            if (_root) Object.Destroy(_root.gameObject);

            _available.Clear();
            _pooled.Clear();
            _root = null;
        }

        private void Prewarm(PoolData data)
        {
            var entries = data.Entries;
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (!entry.prefab)
                    continue;

                var stack = GetStack(entry.prefab);
                for (int n = 0; n < entry.prewarmCount; n++)
                {
                    var instance = Object.Instantiate(entry.prefab, _root);
                    instance.SetActive(false);
                    _pooled[instance] = new Pooled { Prefab = entry.prefab, Component = null, InPool = true };
                    stack.Push(instance);
                }
            }
        }

        private Stack<GameObject> GetStack(GameObject key)
        {
            if (!_available.TryGetValue(key, out var stack))
            {
                stack = new Stack<GameObject>();
                _available[key] = stack;
            }

            return stack;
        }
    }
}
