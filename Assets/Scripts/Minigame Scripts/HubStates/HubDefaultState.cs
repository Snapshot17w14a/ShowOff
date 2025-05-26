using UnityEngine;

public class HubDefaultState : MinigameState
{
    [SerializeField] private MinigameState scoreState;
    [SerializeField] private PlayerDistributor distributor;

    public override void LoadState()
    {
        base.LoadState();
        if (PlayerPrefs.GetInt("DoPodium", 0) == 1) FindFirstObjectByType<MinigameHandler>().LoadState(scoreState);
        else distributor.InstantiatePlayersInCircle(1);
    }
}