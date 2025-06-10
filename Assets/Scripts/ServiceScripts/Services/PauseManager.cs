using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class PauseManager : Service
{
    public Action<bool> OnPaused;
    public static bool isPaused { get; private set; } = false;

    private int pausedByPlayerId = -1;

    private string currentSceneName;

    private EventSystem current;

    private InputSystemUIInputModule currentUIInputModule;

    public override void InitializeService()
    {
        SceneManager.sceneLoaded += (scene, mode) => currentSceneName = scene.name;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void TogglePause(int playerId)
    {
        if (currentSceneName == "HubScene")
        {
            ServiceLocator.GetService<PlayerRegistry>().DisconnectUser(playerId);
            return;
        }

        if (!isPaused && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("HubScene"))
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
        Time.timeScale = 1f;
        isPaused = false;
        pausedByPlayerId = -1;
        ServiceLocator.GetService<PlayerAutoJoin>().AllowJoining = false;
        OnPaused?.Invoke(false);
    }

    public void Pause(int playerId)
    {
        Time.timeScale = 0f;
        isPaused = true;
        ServiceLocator.GetService<PlayerAutoJoin>().AllowJoining = false;
        pausedByPlayerId = playerId;
        OnPaused?.Invoke(true);
    }
}
