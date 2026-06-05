using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Services.EventBusService
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _events = new();

        public static void Subscribe<T>(UnityAction<T> handler) where T : IEvent
        {
            var type = typeof(T);

            if (!_events.ContainsKey(type))
                _events[type] = new List<Delegate>();

            if (_events[type].Contains(handler))
            {
                Debug.LogWarning($"[EventBus] Already subscribed: {type.Name}");
                return;
            }

            _events[type].Add(handler);
        }

        public static void Unsubscribe<T>(UnityAction<T> handler) where T : IEvent
        {
            var type = typeof(T);

            if (!_events.ContainsKey(type) || !_events[type].Remove(handler))
                Debug.LogWarning($"[EventBus] Handler not found to unsubscribe: {type.Name}");
        }

        public static void Publish<T>(T eventData) where T : IEvent
        {
            var type = typeof(T);

            if (!_events.TryGetValue(type, out var handlers) || handlers.Count == 0)
            {
                Debug.LogWarning($"[EventBus] No subscribers for: {type.Name}");
                return;
            }

            foreach (var handler in handlers.ToArray())
                (handler as UnityAction<T>)?.Invoke(eventData);
        }

        public static void Clear<T>() where T : IEvent
        {
            _events.Remove(typeof(T));
        }

        public static void ClearAll()
        {
            _events.Clear();
            Debug.Log("[EventBus] All handlers cleared.");
        }
    }
}