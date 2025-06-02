using UnityEngine;

public class IcyStartState : MinigameState
{
    public void StartGame()
    {
        MinigameHandler.Instance.LoadState(nextMinigameState);
    }
}
