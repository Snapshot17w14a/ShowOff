using System;
using UnityEngine;

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
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
            OnPaused?.Invoke(true);
            pausedByPlayerId = playerId;
        }
        else if (playerId == pausedByPlayerId)
        {
            Time.timeScale = 1f;
            isPaused = false;
            OnPaused?.Invoke(false);
            pausedByPlayerId = -1;
        }
        else
        {
            Debug.Log("You can't pause");
        }
    }
}
