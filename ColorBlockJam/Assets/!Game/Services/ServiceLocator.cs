using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> _services = new();
        
        public static void Register<T>(T service) where T : IService
        {
            var type = typeof(T);

            if (!_services.TryAdd(type, service))
            {
                Debug.LogWarning($"[ServiceLocator] Already registered: {type.Name}");
                return;
            }

            Debug.Log($"[ServiceLocator] Registered: {type.Name}");
        }
        
        
        public static void Unregister<T>() where T : IService
        {
            var type = typeof(T);

            if (!_services.Remove(type))
                Debug.LogWarning($"[ServiceLocator] Not found to unregister: {type.Name}");
            else
                Debug.Log($"[ServiceLocator] Unregistered: {type.Name}");
        }
        
        
        public static T Get<T>() where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;

            Debug.LogError($"[ServiceLocator] Not found: {typeof(T).Name}");
            return default;
        }

        public static bool TryGet<T>(out T service) where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var raw))
            {
                service = (T)raw;
                return true;
            }

            service = default;
            return false;
        }
    }
}