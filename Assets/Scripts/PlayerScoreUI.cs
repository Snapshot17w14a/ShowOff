using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private TreasureInteraction treasureInteraction;
    private MinigamePlayer minigamePlayer;

    private int score;

    public void Initialize(TreasureInteraction _treasureInteraction, MinigamePlayer player)
    {
        treasureInteraction = _treasureInteraction;
        minigamePlayer = player;
        treasureInteraction.OnTreasureDelivered += OnTreasureDelivered;
        UpdateText();
    }

    private void OnDestroy()
    {
        treasureInteraction.OnTreasureDelivered -= OnTreasureDelivered;
    }

    private void OnTreasureDelivered()
    {
        score++;
        ServiceLocator.GetService<ScoreRegistry>().AddScore(minigamePlayer, 1);
        UpdateText();
    }

    private void UpdateText()
    {
        scoreText.text = score.ToString();
    }
}
