using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Events
{
    public class DispatcherService : IDispatcherService
    {
        private readonly Dictionary<Type, Delegate> _eventListeners = new();

        public void Subscribe<T>(Action<T> listener) where T : GameEventBase
        {
            if (!_eventListeners.ContainsKey(typeof(T)))
            { 
                _eventListeners[typeof(T)] = listener;
            }
            else
            {
                _eventListeners[typeof(T)] =
                    Delegate.Combine(_eventListeners[typeof(T)],
                        listener);
            }
        }

        public void Unsubscribe<T>(Action<T> listener) where T : GameEventBase
        {
            if (_eventListeners.ContainsKey(typeof(T)))
            {
                _eventListeners[typeof(T)] = Delegate.Remove(_eventListeners[typeof(T)], listener);

                // If no listeners remain, remove the key from the dictionary
                if (_eventListeners[typeof(T)] == null)
                {
                    _eventListeners.Remove(typeof(T));
                }
            }
        }

        public void Dispatch(GameEventBase eventData)
        {
            Type eventType = eventData.GetType();

            if (_eventListeners.TryGetValue(eventType, out Delegate handler))
            {
                handler?.DynamicInvoke(eventData); // Invoke all registered listeners for this event type
            }
            else
            {
                Debug.LogWarning($"No listeners found for event: {eventType}");
            }
        }

        public void ClearAllSubscriptions()
        {
            _eventListeners.Clear();
        }
    }
}