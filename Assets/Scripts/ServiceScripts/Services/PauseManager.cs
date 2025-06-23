using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : Service
{
    public Action<bool, int> OnPaused;
    public static bool IsPaused { get; private set; } = false;

    public InputDevice PauseingUserDevice => Services.Get<PlayerRegistry>().GetPlayerData(pausedByPlayerId).device;

    private int pausedByPlayerId = -1;

    private string currentSceneName;

    public override void InitializeService()
    {
        SceneManager.sceneLoaded += (scene, mode) => currentSceneName = scene.name;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void TogglePause(int playerId)
    {
        if (currentSceneName == "HubScene")
        {
            Services.Get<PlayerRegistry>().DisconnectUser(playerId);
            return;
        }

        if (!IsPaused && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("HubScene"))
        {
            Pause(playerId);
        }
        else if (playerId == pausedByPlayerId)
        {
            Unpause();
        }
        else
        {
            Debug.Log("You can't pause");
        }
    }

    public void Unpause()
    {
        OnPaused?.Invoke(false, pausedByPlayerId);
        Time.timeScale = 1f;
        IsPaused = false;
        pausedByPlayerId = -1;
        Services.Get<PlayerAutoJoin>().AllowJoining = false;
        Services.Get<PlayerRegistry>().ExecuteForEachPlayer(player => player.SetInputEnabled(true));
    }

    public void Pause(int playerId)
    {
        Time.timeScale = 0f;
        IsPaused = true;
        Services.Get<PlayerAutoJoin>().AllowJoining = false;
        Services.Get<PlayerRegistry>().ExecuteForEachPlayer(player => player.SetInputEnabled(false));
        pausedByPlayerId = playerId;
        OnPaused?.Invoke(true, playerId);
    }
}
