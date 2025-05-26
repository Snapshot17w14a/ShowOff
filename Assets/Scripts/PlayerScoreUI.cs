using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] private Image[] imagesToRecolor;

    [SerializeField] private TMP_Text scoreText;
    private TreasureInteraction treasureInteraction;

    private MinigamePlayer Player => minigamePlayer;
    private MinigamePlayer minigamePlayer;

    public int Score => score;
    private int score;

    public void Initialize(TreasureInteraction _treasureInteraction, MinigamePlayer player)
    {
        treasureInteraction = _treasureInteraction;
        minigamePlayer = player;
        treasureInteraction.OnTreasureDelivered += OnTreasureDelivered;
        UpdateText();
        SetColor(player.playerColor);
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

    public void SetColor(Color color)
    {
        foreach (var img in imagesToRecolor) img.color = color;
    }
}
