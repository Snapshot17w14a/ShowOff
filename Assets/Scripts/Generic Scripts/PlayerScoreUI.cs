using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour
{
    public event Action OnEvaluateScore;

    [SerializeField] private Image[] imagesToRecolor;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] public GameObject goldenPenguinFrame;
    [SerializeField] public GameObject fireAnimation;


    [SerializeField] private float increasedFontSize;
    private float initialFontSize;

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
        SetColor(player.GetComponent<SkinManager>().playerColor);
        initialFontSize = scoreText.fontSize;
    }

    private void OnDestroy()
    {
        treasureInteraction.OnTreasureDelivered -= OnTreasureDelivered;
    }

    private void OnTreasureDelivered(int points)
    {
        score += points;
        ServiceLocator.GetService<ScoreRegistry>().AddScore(minigamePlayer, points);
        UpdateText();
        Scheduler.Instance.Lerp(t =>
        {
            scoreText.fontSize = Mathf.Lerp(initialFontSize, increasedFontSize, t * t);
        }, 0.25f, () => Scheduler.Instance.Lerp(t =>
        {
            scoreText.fontSize = Mathf.Lerp(increasedFontSize, initialFontSize, t * t);
        }, 0.25f));
        OnEvaluateScore?.Invoke();
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
