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

    public override void LoadState()
    {
        base.LoadState();
        Scheduler.Instance.DelayExecution(PlayCountDown, stateDurationSeconds - 10);
    }

    private void PlayCountDown()
    {
        AudioManager.PlaySound(ESoundType.Other, "Icy_Showdown_CountDown_10", false);
    }
}
