using UnityEngine;
using UnityEngine.Video;

public class HubIdleState : MinigameState
{
    [SerializeField] private VideoPlayer idleVideoPlayer;

    public override void LoadState()
    {
        base.LoadState();
        idleVideoPlayer.Play();
    }

    public override void TickState()
    {
        base.TickState();
        if (Input.anyKey)
        {
            idleVideoPlayer.Pause();
            FindFirstObjectByType<MinigameHandler>().LoadState(nextMinigameState);
        }
    }

    public override void UnloadState()
    {
        base.UnloadState();
    }
}
