using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    [SerializeField] private Transform scoreParent;
    [SerializeField] private PlayerScoreUI scorePrefab;
    [SerializeField] private Material gold;
    [SerializeField] private int fireAnimationNumber = 3;

    private Dictionary<MinigamePlayer, PlayerScoreUI> scores = new();

    public static PlayerScoreManager Instance => _instance;
    private static PlayerScoreManager _instance;
    private PlayerScoreUI playerScore;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        Debug.Assert(scoreParent != null);
        Debug.Assert(scorePrefab != null);

        ServiceLocator.GetService<PlayerRegistry>().OnPlayerSpawn += GenerateScoreUI;
    }

    private void OnDestroy()
    {
        ServiceLocator.GetService<PlayerRegistry>().OnPlayerSpawn -= GenerateScoreUI;
        playerScore.OnEvaluateScore -= ToggleFireUIAnimation;

    }

    private void GenerateScoreUI(MinigamePlayer player)
    {
         playerScore = Instantiate(scorePrefab, scoreParent);
        //Changing the UI frame of the winner penguin
        RegisteredPlayer data = ServiceLocator.GetService<PlayerRegistry>().GetPlayerData(player.RegistryID);
        if (data.isLastWinner)
        {
            playerScore.goldenPenguinFrame.SetActive(true);
        }

        playerScore.Initialize(player.TreasureInteraction, player);
        playerScore.gameObject.SetActive(true);
        playerScore.OnEvaluateScore += ToggleFireUIAnimation;
        scores.Add(player, playerScore);
    }

    private void ToggleFireUIAnimation()
    {
        if (scores.Count < 2) return;

        List<int> allScores = new();
        List<KeyValuePair<MinigamePlayer, PlayerScoreUI>> sorted = new(scores);
        sorted.Sort((a, b) => b.Value.Score.CompareTo(a.Value.Score));

        int topScore = sorted[0].Value.Score;
        int secondScore = sorted[1].Value.Score;

        bool shouldEnableFire = topScore - secondScore >= fireAnimationNumber;
        for (int i = 0; i < sorted.Count; i++)
        {
            var playerUI = sorted[i].Value;
            bool enableFire = (i == 0 && shouldEnableFire);
            playerUI.fireAnimation.SetActive(enableFire);
        }
    }

    public PlayerScoreUI GetPlayerUI(MinigamePlayer player) => scores[player];
}
