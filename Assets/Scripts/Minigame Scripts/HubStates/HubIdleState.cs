using UnityEngine;
using UnityEngine.Video;

public class HubIdleState : MinigameState
{
    [SerializeField] private VideoPlayer idleVideoPlayer;
    [SerializeField] private VideoClip[] clips;

    public override void LoadState()
    {
        base.LoadState();
        idleVideoPlayer.clip = clips[Random.Range(0, clips.Length)];
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
