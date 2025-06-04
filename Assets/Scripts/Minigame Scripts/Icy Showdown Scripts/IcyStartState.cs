using UnityEngine;

public class IcyStartState : MinigameState
{
#if UNITY_EDITOR
    public override void UnloadState()
    {
        base.UnloadState();
        ServiceLocator.GetService<PlayerAutoJoin>().AllowJoining = true;
    }
#endif

    public void StartGame()
    {
        MinigameHandler.Instance.LoadState(nextMinigameState);
    }
}
