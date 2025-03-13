using System;

namespace Services.Events
{
    public interface IDispatcherService
    {
        public void Subscribe<T>(Action<T> handler) where T : GameEventBase;
        public void Unsubscribe<T>(Action<T> handler) where T : GameEventBase;
        public void Dispatch(GameEventBase gameEventBase);
        public void ClearAllSubscriptions();
    }
}