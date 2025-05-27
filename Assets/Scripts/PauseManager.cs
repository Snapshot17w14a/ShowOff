using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : Service
{
    public Action<bool> OnPaused;
    public bool isPaused { get; private set; } = false;

    private int pausedByPlayerId = -1;

    public override void InitializeService()
    {

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
            Time.timeScale = 0f;
            isPaused = true;
            OnPaused?.Invoke(true);
            pausedByPlayerId = playerId;
            ServiceLocator.GetService<PlayerAutoJoin>().AllowJoining = false;
        }
        else if (playerId == pausedByPlayerId)
        {
            Time.timeScale = 1f;
            isPaused = false;
            OnPaused?.Invoke(false);
            pausedByPlayerId = -1;
            ServiceLocator.GetService<PlayerAutoJoin>().AllowJoining = false;

        }
        else
        {
            Debug.Log("You can't pause");
        }
    }

    public void SetIsPaused()
    {
        Time.timeScale = 1f;
        OnPaused?.Invoke(false);
        ServiceLocator.GetService<PlayerAutoJoin>().AllowJoining = false;
        isPaused = !isPaused;
    }
}
