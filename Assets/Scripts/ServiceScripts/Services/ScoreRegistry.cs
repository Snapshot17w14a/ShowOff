using System.Collections.Generic;

public class ScoreRegistry : Service
{
    private Dictionary<int, int> playerScores;

    public override void InitializeService()
    {
        playerScores = new();
    }

    public void AddScore(int idOfPlayer, int scoreToAdd) => playerScores[idOfPlayer] += scoreToAdd;

    public void AddScore(MinigamePlayer player, int scoreToAdd) => playerScores[ServiceLocator.GetService<PlayerRegistry>().IdOf(player)] += scoreToAdd;

    public int ScoreOfPlayer(int idOfPlayer) => playerScores[idOfPlayer];

    public int ScoreOfPlayer(MinigamePlayer player) => playerScores[ServiceLocator.GetService<PlayerRegistry>().IdOf(player)];

    public void WipeData() => playerScores.Clear();
}