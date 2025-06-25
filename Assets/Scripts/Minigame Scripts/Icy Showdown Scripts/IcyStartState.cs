public class IcyStartState : MinigameState
{
    public override void UnloadState()
    {
        base.UnloadState();
#if UNITY_EDITOR
        Services.Get<PlayerAutoJoin>().AllowJoining = true;
#endif
    }

    public void StartGame()
    {
        MinigameHandler.Instance.LoadState(nextMinigameState);
    }
}
