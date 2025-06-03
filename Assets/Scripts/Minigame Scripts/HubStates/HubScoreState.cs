using UnityEngine;

public class HubScoreState : MinigameState
{
    [SerializeField] private float fadeOutSeconds = 2f;
    [SerializeField] private RendererFading fader;

    public override void LoadState()
    {
        base.LoadState();
        fader.FadeAllChildOut(fadeOutSeconds, stateDurationSeconds - fadeOutSeconds);
    }

    public override void UnloadState()
    {
        base.UnloadState();
        ((HubDefaultState)nextMinigameState).isAfterPodiums = true;
    }
}
