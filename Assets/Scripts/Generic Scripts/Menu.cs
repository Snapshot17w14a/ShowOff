using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private PlayerRegistry playerRegistry;

    private readonly List<MinigamePlayer> players = new List<MinigamePlayer>();

    private void Awake()
    {
        playerRegistry = ServiceLocator.GetService<PlayerRegistry>();
        playerRegistry.OnPlayerSpawn += HandlePlayerSpawned;
        playerRegistry.BeforePlayerDisconnect += HandlePlayerDisconnected;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        playerRegistry.OnPlayerSpawn -= HandlePlayerSpawned;
        playerRegistry.BeforePlayerDisconnect -= HandlePlayerDisconnected;

        foreach (MinigamePlayer player in players)
        {
            player.OnPlayerPaused -= HandlePlayerPaused;
        }

        players.Clear();
    }

    public void Continue()
    {
        ServiceLocator.GetService<PauseManager>().Unpause();

        foreach (MinigamePlayer player in players)
        {
            player.SetInputEnabled(true);
        }
    }

    public void Restart()
    {
        ServiceLocator.GetService<PauseManager>().Unpause();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void GoToHub()
    {
        ServiceLocator.GetService<PauseManager>().Unpause();
        SceneManager.LoadScene("HubScene");
    }
    private void HandlePlayerPaused(int registryId)
    {
        foreach(MinigamePlayer player in players)
        {
            player.SetInputEnabled(player.RegistryID == registryId);
        }
    }

    private void HandlePlayerSpawned(MinigamePlayer player)
    {
        player.OnPlayerPaused += HandlePlayerPaused;
        players.Add(player);
    }

    private void HandlePlayerDisconnected(MinigamePlayer disconnectingPlayer)
    {
        disconnectingPlayer.OnPlayerPaused -= HandlePlayerPaused;
        players.Remove(disconnectingPlayer);
    }
}
