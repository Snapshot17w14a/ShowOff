using UnityEngine;

public class HubScoreState : MinigameState
{
    [SerializeField] private float fadeOutSeconds = 2f;

    public override void LoadState()
    {
        base.LoadState();
    }

    public override void UnloadState()
    {
        base.UnloadState();
        ((HubDefaultState)nextMinigameState).isAfterPodiums = true;
    }

    public void SkipPodiumStage()
    {
        FindFirstObjectByType<MinigameHandler>().LoadState(nextMinigameState);
    }
}
