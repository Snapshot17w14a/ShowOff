public class IcyStartState : MinigameState
{
    public override void UnloadState()
    {
#if UNITY_EDITOR
        base.UnloadState();
        Services.Get<PlayerAutoJoin>().AllowJoining = true;
#endif
    }

    public void StartGame()
    {
        MinigameHandler.Instance.LoadState(nextMinigameState);
    }
}
