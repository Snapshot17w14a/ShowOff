using System;

public class EventBus<T> where T : GlobalEvent
{
    public static event Action<T> OnEvent;

    public static void RaiseEvent(T Event)
    {
        OnEvent?.Invoke(Event);
    }
}

public abstract class GlobalEvent { }

public class PickupCollected : GlobalEvent
{
    public Pickupable pickup;

    public PickupCollected(Pickupable pickupable)
    {
        pickup = pickupable;
    }
}

public class SceneRestart : GlobalEvent { }
