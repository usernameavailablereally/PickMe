using MonoBehaviourComponents;

namespace Services.Events
{
    public class UnknownItemClickedEvent : GameEventBase
    {
        public ItemComponent Component { get; private set; }
        public UnknownItemClickedEvent(ItemComponent component)
        {
            Component = component;
        }
    }
    public class TargetItemClickedEvent : GameEventBase
    {
        public ItemComponent Component { get; private set; }
        public TargetItemClickedEvent(ItemComponent component)
        {
            Component = component;
        }
    } 
}