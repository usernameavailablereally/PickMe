using System;
using System.Collections.Generic;

namespace Services.Events
{
    public class DispatcherService : IDispatcherService
    { 
        private readonly Dictionary<Type, List<Action<GameEventBase>>> _eventHandlers = new();

        public void Subscribe<T>(Action<T> handler) where T : GameEventBase
        {
            Type eventType = typeof(T);
            if (!_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType] = new List<Action<GameEventBase>>();
            }

            _eventHandlers[eventType].Add((e) => handler((T)e));
        }

        public void Unsubscribe<T>(Action<T> handler) where T : GameEventBase
        {
            Type eventType = typeof(T);
            if (!_eventHandlers.TryGetValue(eventType, out List<Action<GameEventBase>> eventHandler)) return;

            eventHandler.RemoveAll(h => h.Target == handler.Target && h.Method == handler.Method);
        }

        public void Dispatch(GameEventBase gameEventBase)
        {
            Type eventType = gameEventBase.GetType();
            if (!_eventHandlers.TryGetValue(eventType, out List<Action<GameEventBase>> eventHandler)) return;

            // TODO - This is a workaround to avoid modifying the list while iterating over it.
            var handlers = eventHandler.ToArray();

            foreach (Action<GameEventBase> handler in handlers)
            {
                try
                {
                    handler(gameEventBase);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"Error handling event {eventType}: {e}");
                }
            }
        }

        public void ClearAllSubscriptions()
        {
            _eventHandlers.Clear();
        }
    }
}
