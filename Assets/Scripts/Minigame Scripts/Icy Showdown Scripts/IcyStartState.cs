public class IcyStartState : MinigameState
{
#if UNITY_EDITOR
    public override void UnloadState()
    {
        base.UnloadState();
        Services.Get<PlayerAutoJoin>().AllowJoining = true;
    }
#endif

    public void StartGame()
    {
        MinigameHandler.Instance.LoadState(nextMinigameState);
    }
}
