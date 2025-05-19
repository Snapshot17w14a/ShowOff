using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private TreasureInteraction treasureInteraction;

    private int score;

    public void Initialize(TreasureInteraction _treasureInteraction)
    {
        treasureInteraction = _treasureInteraction;
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
        UpdateText();
    }

    private void UpdateText()
    {
        scoreText.text = score.ToString();
    }
}
