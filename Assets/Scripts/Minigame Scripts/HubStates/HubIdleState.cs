using UnityEngine;
using UnityEngine.Video;

public class HubIdleState : MinigameState
{
    [SerializeField] private VideoPlayer idleVideoPlayer;
    [SerializeField] private VideoClip[] clips;
    private bool isTransitioning = false;

    public override void LoadState()
    {
        base.LoadState();
        idleVideoPlayer.clip = clips[Random.Range(0, clips.Length)];
        idleVideoPlayer.Play();
    }

    public override void TickState()
    {
        base.TickState();
        if (Input.anyKey && !isTransitioning)
        {
            isTransitioning = true;
            TransitionController.Instance.TransitionOut(1f, () =>
            {
                isTransitioning = false;
                idleVideoPlayer.Pause();
                FindFirstObjectByType<MinigameHandler>().LoadState(nextMinigameState);
                TransitionController.Instance.TransitionIn(1f);
            });
        }
    }

    public override void UnloadState()
    {
        base.UnloadState();
    }
}
