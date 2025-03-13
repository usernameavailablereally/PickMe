using MonoBehaviours;

namespace Services.Events
{
    public class UIButtonClickEvent : GameEventBase
    {
    }

    public class ItemClickedEvent : GameEventBase
    {
        public ItemController Controller { get; private set; }
        public ItemClickedEvent(ItemController controller)
        {
            Controller = controller;
        }
    }

    public class TargetItemClickedEvent : GameEventBase
    {
        public ItemController Controller { get; private set; }
        public TargetItemClickedEvent(ItemController controller)
        {
            Controller = controller;
        }
    }

    public class WrongItemClickedEvent : GameEventBase
    {
        public ItemController Controller { get; private set; }
        public WrongItemClickedEvent(ItemController controller)
        {
            Controller = controller;
        }
    }
}