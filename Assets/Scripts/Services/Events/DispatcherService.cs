using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Services.Events
{
    public class DispatcherService : IDispatcherService, IDisposable
    {
        /// Thread-safe dictionary
        private readonly ConcurrentDictionary<Type, Delegate> _eventListeners = new();
        private readonly ConcurrentDictionary<Type, byte> _processingEvents = new();

        public void Subscribe<T>(Action<T> listener) where T : GameEventBase
        {
            _eventListeners.AddOrUpdate(
                typeof(T),
                listener,
                (_, existing) => Delegate.Combine(existing, listener));
        }

        public void Unsubscribe<T>(Action<T> listener) where T : GameEventBase
        {
            _eventListeners.AddOrUpdate(
                typeof(T),
                _ => null,
                (_, existing) => Delegate.Remove(existing, listener));
        }

        public void Dispatch(GameEventBase eventData)
        {
            var eventType = eventData.GetType();
            if (!_processingEvents.TryAdd(eventType, 0)) return;

            try
            {
                if (_eventListeners.TryGetValue(eventType, out var handler))
                {
                    foreach (var d in handler.GetInvocationList())
                    {
                        try
                        {
                            d.DynamicInvoke(eventData);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Handler error: {e.Message}");
                        }
                    }
                }
            }
            finally
            {
                _processingEvents.TryRemove(eventType, out _);
            }
        }

        public void ClearAllSubscriptions()
        {
            _eventListeners.Clear();
        }

        public void Dispose()
        {
            _eventListeners.Clear();
            _processingEvents.Clear();
            GC.SuppressFinalize(this);
        }

        ~DispatcherService() => Dispose();
    }
}