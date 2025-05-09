using System;

public class EventBus<T> where T : Event
{
    public static event Action<T> OnEvent;

    public static void RaiseEvent(T Event)
    {
        OnEvent?.Invoke(Event);
    }
}

public abstract class Event { }
