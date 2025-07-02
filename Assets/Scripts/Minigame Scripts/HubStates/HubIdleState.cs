using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class HubIdleState : MinigameState
{
    [SerializeField] private VideoPlayer idleVideoPlayer;
    [SerializeField] private VideoClip[] clips;
    private bool isTransitioning = false;

    private int currentIndex = -1;

    private readonly List<InputDevice> inputDevices = new();

    private void Start()
    {
        Services.Get<PlayerRegistry>().OnPlayerRegistered += RepopulateDeviceList;
    }

    public override void LoadState()
    {
        base.LoadState();
        Services.Get<PlayerRegistry>().ExecuteForEachPlayer(p => Destroy(p.gameObject));
        Services.Get<PlayerAutoJoin>().AutoJoinPlayers = false;
        StartRandomClip(idleVideoPlayer);
        idleVideoPlayer.Play();
        idleVideoPlayer.loopPointReached += StartRandomClip;
    }

    public override void TickState()
    {
        base.TickState();
        if (AnyInput() && !isTransitioning)
        {
            isTransitioning = true;
            TransitionController.Instance.TransitionOut(1f, () =>
            {
                isTransitioning = false;
                idleVideoPlayer.Pause();
                MinigameHandler.Instance.LoadState(nextMinigameState);
                TransitionController.Instance.TransitionIn(1f);
            });
        }
    }

    public override void UnloadState()
    {
        base.UnloadState();
        Services.Get<PlayerAutoJoin>().AutoJoinPlayers = true;
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

    private void RepopulateDeviceList(int id)
    {
        inputDevices.Clear();
        inputDevices.AddRange(InputSystem.devices);
    }

    private bool AnyInput()
    {
        foreach (var device in inputDevices)
            if (device.IsPressed()) return true;
        return Input.anyKey;
    }

    private void OnDisable()
    {
        Services.Get<PlayerRegistry>().OnPlayerRegistered -= RepopulateDeviceList;
    }
}
