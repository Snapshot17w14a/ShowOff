using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    [SerializeField] private Transform scoreParent;
    [SerializeField] private PlayerScoreUI scorePrefab;
    [SerializeField] private Material gold;

    private Dictionary<MinigamePlayer, PlayerScoreUI> scores = new();

    public static PlayerScoreManager Instance => _instance;
    private static PlayerScoreManager _instance;



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
    }

    private void GenerateScoreUI(MinigamePlayer player)
    {
        PlayerScoreUI playerScore = Instantiate(scorePrefab, scoreParent);

        //Changing the UI frame of the winner penguin
        RegisteredPlayer data = ServiceLocator.GetService<PlayerRegistry>().GetPlayerData(player.RegistryID);
        if(data.isLastWinner)
        {
            playerScore.goldenPenguinFrame.gameObject.SetActive(true);
        }

        playerScore.Initialize(player.TreasureInteraction, player);
        playerScore.gameObject.SetActive(true);
        scores.Add(player, playerScore);
    }

    public PlayerScoreUI GetPlayerUI(MinigamePlayer player) => scores[player];
}
