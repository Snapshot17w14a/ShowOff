using System.Collections;
using UnityEngine;

public class PodiumController : MonoBehaviour
{
    [SerializeField] private GameObject podiumPrefab;
    [SerializeField] private float spacing;

    [SerializeField] private int scorePerSecond;

    private int currentScore;
    private int targetScore;

    private Podium[] podiums;

    public void StartPodiumSequence()
    {
        //Get a referene to the PlayerRegistry and get player count
        var registry = ServiceLocator.GetService<PlayerRegistry>();
        int playerCount = registry.RegisteredPlayerCount;

        //Get the highest score from the ScoreRegistry
        PlayerScore highestScore = ServiceLocator.GetService<ScoreRegistry>().HighestScore;
        Podium.HighestScore = highestScore.score;

        //Reset all winner flags to false
        registry.ExecuteForEachPlayerData(data =>
        {
            data.isLastWinner = false;
            return data;
        }); 

        //Set the winner's data to have winner as true
        var winnerData = registry.GetPlayerData(highestScore.id);
        winnerData.isLastWinner = true;
        registry.SetPlayerData(winnerData);

        podiums = new Podium[playerCount];

        //Create a podium for all players
        for (int i = 0; i < playerCount; i++)
        {
            var podium = CreatePodium(i, registry);
            podiums[i] = podium;
        }

        //Set the position to center all the podiums
        var parentPos = transform.position;
        parentPos.x = -((playerCount + spacing * Mathf.Max(playerCount - 1, 0)) / 2f);
        transform.position = parentPos;

        //Reset the podium flag in the PlayerPrefs
        PlayerPrefs.SetInt("DoPodium", 0);

        //Set up the Animation and start it
        targetScore = highestScore.score;
        currentScore = 0;
        StartCoroutine(AnimatePodiums(new WaitForSeconds(1 / (float)scorePerSecond)));
    }

    //Create a podium, set its position based on the index
    private Podium CreatePodium(int index, PlayerRegistry registry)
    {
        var pos = new Vector3(0.5f * (index + 1) + (0.5f + spacing) * index, 0, 0);
        var podium = Instantiate(podiumPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Podium>();
        podium.transform.localPosition = pos;
        podium.player = registry.InstantiatePlayerWithId(index);

        return podium;
    }

    private IEnumerator AnimatePodiums(WaitForSeconds waitCondition)
    {
        while (currentScore <= targetScore)
        {
            foreach (var podium in podiums)
            {
                podium.UpdateScaleAndPosition(currentScore);
            }

            currentScore++;

            yield return waitCondition;
        }

        Podium.HighestScore = -1;
    }
}
