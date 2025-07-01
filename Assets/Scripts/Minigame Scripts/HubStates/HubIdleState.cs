using UnityEngine;
using UnityEngine.Video;

public class HubIdleState : MinigameState
{
    [SerializeField] private VideoPlayer idleVideoPlayer;
    [SerializeField] private VideoClip[] clips;
    private bool isTransitioning = false;

    private int currentIndex = -1;

    public override void LoadState()
    {
        base.LoadState();
        Services.Get<PlayerRegistry>().ExecuteForEachPlayer(p => Destroy(p.gameObject));
        StartRandomClip(idleVideoPlayer);
        idleVideoPlayer.Play();
        idleVideoPlayer.loopPointReached += StartRandomClip;
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
        idleVideoPlayer.loopPointReached -= StartRandomClip;
    }

    private void StartRandomClip(VideoPlayer source)
    {
        int i;
        do { i = Random.Range(0, clips.Length); }
        while (i == currentIndex);
        source.clip = clips[i];
        currentIndex = i;
    }
}
