using UnityEngine;

public class HubDefaultState : MinigameState
{
    [SerializeField] private MinigameState scoreState;
    [SerializeField] private PlayerDistributor distributor;
    [SerializeField] private Camera mainCamera;

    [HideInInspector] public bool isAfterPodiums = false;

    public override void LoadState()
    {
        base.LoadState();
        if (PlayerPrefs.GetInt("DoPodium", 0) == 1) FindFirstObjectByType<MinigameHandler>().LoadState(scoreState);
        else if (!isAfterPodiums) distributor.InstantiatePlayersInCircle(1);
    }
}