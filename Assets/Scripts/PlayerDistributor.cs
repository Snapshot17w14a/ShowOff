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

        foreach(var player in playerRegistry.AllPlayers)
        {
            if (player == null) continue;
            player.transform.position = new Vector3(Mathf.Cos(t * Tau), 1, Mathf.Sin(t * Tau)) * radius;
            t += increment;
        }
    }
}
