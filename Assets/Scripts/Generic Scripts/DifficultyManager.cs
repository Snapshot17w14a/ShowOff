using UnityEngine;

public enum GameDifficulty
{
    Easy,
    Normal
}

public static class DifficultyManager
{
    private static GameDifficulty currentDifficulty = GameDifficulty.Normal;

    public static void SwitchDifficulty()
    {
        if(currentDifficulty == GameDifficulty.Easy)
        {
            currentDifficulty = GameDifficulty.Normal;
        } else
        {
            currentDifficulty = GameDifficulty.Easy;
        }
        Debug.Log($"Game difficulty set to: {currentDifficulty}");
    }

    public static void SetDifficulty(GameDifficulty difficulty)
    {
        currentDifficulty = difficulty;
        Debug.Log($"Game difficulty set to: {currentDifficulty}");
    }

    public static GameDifficulty GetDifficulty()
    {
        return currentDifficulty;
    }
    public static bool IsEasyMode()
    {
        return currentDifficulty == GameDifficulty.Easy;
    }
    public static bool IsNormalMode()
    {
        return currentDifficulty == GameDifficulty.Normal;
    }
}
