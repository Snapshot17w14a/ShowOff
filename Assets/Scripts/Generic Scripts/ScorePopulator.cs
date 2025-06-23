using System.Linq;
using TMPro;
using UnityEngine;

public class ScorePopulator : MonoBehaviour
{
    [SerializeField] private GameObject scoreEntryPrefab;

    public void PopulateScoreBoard()
    {
        var scores = Services.Get<ScoreRegistry>().GetStoredScores();

        var sortedScores = scores.OrderByDescending(score => score.score).ToArray();

        foreach (var score in sortedScores)
        {
            var entry = Instantiate(scoreEntryPrefab, transform);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = $"Player {score.id + 1} - Score: {score.score}";
        }
    }
}
