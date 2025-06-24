public class IcyGameState : MinigameState
{
    public override void TickState()
    {
        base.TickState();
    }

    public override void UnloadState()
    {
        base.UnloadState();
        Scheduler.Instance.StopAllRoutines();
    }
}
