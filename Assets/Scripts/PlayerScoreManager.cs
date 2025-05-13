using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    [SerializeField] private CreatePlayer createPlayer;
    [SerializeField] private Transform scoreParent;
    [SerializeField] private PlayerScoreUI scorePrefab;
    [SerializeField] private float xOffset = 100f;

    private List<PlayerScoreUI> scores = new List<PlayerScoreUI>();

    private void Awake()
    {
        Debug.Assert(createPlayer != null);
        Debug.Assert(scoreParent != null);
        Debug.Assert(scorePrefab != null);

        ServiceLocator.GetService<PlayerRegistry>().OnPlayerSpawn += GenerateScoreUI;
    }

    private void OnDestroy()
    {
        ServiceLocator.GetService<PlayerRegistry>().OnPlayerSpawn -= GenerateScoreUI;
    }

    private void GenerateScoreUI(MinigamePlayer player)
    {
        PlayerScoreUI playerScore = Instantiate(scorePrefab, scoreParent);
        playerScore.Initialize(player.TreasureInteraction);
        playerScore.gameObject.SetActive(true);
        scores.Add(playerScore);
    }
}
