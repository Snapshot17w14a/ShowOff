using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HubDefaultState : MinigameState
{
    [SerializeField] private MinigameState scoreState;
    [SerializeField] private PlayerDistributor distributor;
    [SerializeField] private Camera mainCamera;

    [HideInInspector] public bool isAfterPodiums = false;

    [SerializeField] private Volume volume;

    public override void LoadState()
    {
        base.LoadState();
        SetVignetteToZero();
        Services.Get<PlayerAutoJoin>().AllowJoining = true;
        Services.Get<PlayerAutoJoin>().AutoJoinPlayers = true;
        if (PlayerPrefs.GetInt("DoPodium", 0) == 1) FindFirstObjectByType<MinigameHandler>().LoadState(scoreState);
        else if (!isAfterPodiums) distributor.InstantiatePlayersInCircle(1);
    }

    public void SetVignetteToZero()
    {
        volume.sharedProfile.TryGet<ChromaticAberration>(out var chromatic);
        chromatic.intensity.value = 0;
    }
}