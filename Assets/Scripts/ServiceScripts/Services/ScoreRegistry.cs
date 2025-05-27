using System.Collections.Generic;

public class ScoreRegistry : Service
{
    //ID, Score pair
    private Dictionary<int, int> playerScores;

    public override void InitializeService()
    {
        playerScores = new();
    }

    public void AddPlayer(int idOfPlayer) { if (!playerScores.ContainsKey(idOfPlayer)) playerScores.Add(idOfPlayer, 0); }

    public void AddPlayer(MinigamePlayer player) => AddPlayer(player.RegistryID);

    public void AddScore(int idOfPlayer, int scoreToAdd)
    {
        if (!playerScores.ContainsKey(idOfPlayer)) playerScores.Add(idOfPlayer, scoreToAdd);
        else playerScores[idOfPlayer] += scoreToAdd;
    }

    public void AddScore(MinigamePlayer player, int scoreToAdd) => AddScore(player.RegistryID, scoreToAdd);

    public int ScoreOfPlayer(int idOfPlayer)
    {
        if (!playerScores.ContainsKey(idOfPlayer)) return 0;
        return playerScores[idOfPlayer];
    }

    public int ScoreOfPlayer(MinigamePlayer player) => ScoreOfPlayer(player.RegistryID);

    public PlayerScore[] GetStoredScores()
    {
        var scores = new PlayerScore[playerScores.Count];

        int index = 0;
        foreach (var score in playerScores)
        {
            scores[index] = new(score.Key, score.Value);
            index++;
        }

        return scores;
    }

    public PlayerScore HighestScore
    {
        get
        {
            int highestScore = 0;
            int highestScoreId = 0;
            bool isUnique = true;

            foreach (var score in playerScores)
            {
                if (score.Value > highestScore)
                {
                    highestScore = score.Value;
                    highestScoreId = score.Key;
                    isUnique = true;
                }
                else if (score.Value == highestScore) isUnique = false;
            }

            return new PlayerScore(highestScoreId, highestScore) { isUnique = isUnique };
        }
    }

    public void WipeData() => playerScores.Clear();
}

public struct PlayerScore
{
    public PlayerScore(int id, int score)
    {
        this.id = id;
        this.score = score;
        isUnique = true;
    }

    public int id;
    public int score;
    public bool isUnique;
}