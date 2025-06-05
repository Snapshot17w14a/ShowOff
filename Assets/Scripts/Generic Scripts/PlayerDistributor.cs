using UnityEngine;

public class PlayerDistributor : MonoBehaviour
{
    private const float Tau = 2 * Mathf.PI;

    public void InstantiatePlayersInCircle(float radius)
    {
        var playerRegistry = ServiceLocator.GetService<PlayerRegistry>();
        playerRegistry.InstantiateAllPlayers();

        int playerCount = playerRegistry.RegisteredPlayerCount;
        float increment = 1 / (float)playerCount;
        float t = increment;

        foreach (var player in playerRegistry.AllPlayers)
        {
            if (RegisteredPlayer.IsNull(player)) continue;

            if (player.isLastWinner)
            {
                player.minigamePlayer.ChangeSkin();
            }

            var minigamePlayer = player.minigamePlayer;
            minigamePlayer.transform.position = new Vector3(Mathf.Cos(t * Tau), 1, Mathf.Sin(t * Tau)) * radius;

            t += increment;
        }
    }

    public void SetOnPodiumFlag(bool set) => PlayerPrefs.SetInt("DoPodium", set ? 1 : 0);
}
