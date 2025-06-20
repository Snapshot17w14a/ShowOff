using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    [SerializeField] private Transform scoreParent;
    [SerializeField] private PlayerScoreUI scorePrefab;
    [SerializeField] private Material gold;
    [SerializeField] private GameObject fireAnimation;
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

        foreach (var kvp in scores)
        {
            allScores.Add(kvp.Value.Score);
        }

        allScores.Sort((a, b) => b.CompareTo(a));

        int topScore = allScores[0];
        int secondScore = allScores[1];

        if(topScore - secondScore >= fireAnimationNumber)
        {
            fireAnimation.SetActive(true);
        } else
        {
            fireAnimation.SetActive(false);
        }
    }

    public PlayerScoreUI GetPlayerUI(MinigamePlayer player) => scores[player];
}
