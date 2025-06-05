using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PodiumController : MonoBehaviour
{
    [SerializeField] private GameObject podiumPrefab;
    [SerializeField] private GameObject scoreTextPrefab;
    [SerializeField] private Transform scoreTextParent;

    [SerializeField] private float spacing;
    [SerializeField] private int scorePerSecond;

    public int ScorePerSecond => scorePerSecond;
    public float CooldownPerScore => 1 / (float)scorePerSecond;

    public UnityEvent OnCountStart;
    public UnityEvent OnCountEnd;

    private int currentScore;
    private int targetScore;

    private Podium[] podiums;

    private int winnerID = -1;

    public void StartPodiumSequence()
    {
        //Reset the podium flag in the PlayerPrefs
        PlayerPrefs.SetInt("DoPodium", 0);

        //Get a referene to the PlayerRegistry and get player count
        var registry = ServiceLocator.GetService<PlayerRegistry>();
        int playerCount = registry.RegisteredPlayerCount;

        //Get the highest score from the ScoreRegistry
        PlayerScore highestScore = ServiceLocator.GetService<ScoreRegistry>().HighestScore;
        Podium.highestScore = highestScore.score;
        Podium.controller = this;

        //Reset all winner flags to false
        registry.ExecuteForEachPlayerData(data =>
        {
            data.isLastWinner = false;
            return data;
        });

        //Set the winner's data to have winner as true
        if (highestScore.isUnique)
        {
            var winnerData = registry.GetPlayerData(highestScore.id);
            winnerData.isLastWinner = true;
            winnerID = highestScore.id;
            registry.SetPlayerData(winnerData);
        }

        podiums = new Podium[playerCount];

        //Create a podium for all players
        for (int i = 0; i < playerCount; i++)
        {
            var podium = CreatePodium(i, registry);
            podium.Initialize();
            podiums[i] = podium;
        }

        //Set the position to center all the podiums
        var parentPos = transform.position;
        parentPos.x = -((playerCount + spacing * Mathf.Max(playerCount - 1, 0)) / 2f);
        transform.position = parentPos;

        //Set up the Animation and start it
        targetScore = highestScore.score;
        currentScore = 0;
        OnCountStart?.Invoke();
        StartCoroutine(AnimatePodiums(new WaitForSeconds(CooldownPerScore)));
    }

    //Create a podium, set its position based on the index
    private Podium CreatePodium(int index, PlayerRegistry registry)
    {
        var pos = new Vector3(0.5f * (index + 1) + (0.5f + spacing) * index, 0, 0);
        var podium = Instantiate(podiumPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Podium>();
        podium.transform.localPosition = pos;
        podium.transform.localScale = new Vector3(1, 0, 1);
        podium.player = registry.InstantiatePlayerWithId(index);

        return podium;
    }

    private IEnumerator AnimatePodiums(YieldInstruction waitCondition)
    {
        while (currentScore <= targetScore)
        {
            foreach (var podium in podiums) podium.UpdateScaleAndPosition(currentScore);

            currentScore++;

            yield return waitCondition;
        }

        CleanUp();

        if (winnerID != -1)
            ServiceLocator.GetService<PlayerRegistry>().GetPlayerData(winnerID).minigamePlayer.ChangeSkin();

        OnCountEnd?.Invoke();

        foreach (var podium in podiums) podium.SetPlayerInteraction(true);

        Podium.highestScore = -1;
    }

    public GameObject CreateScoreText()
    {
        var scoreText = Instantiate(scoreTextPrefab, scoreTextParent);
        return scoreText;
    }

    public void CleanUp()
    {
        foreach (var podium in podiums)
        {
            podium.CleanUp();
            Destroy(podium);
        }
    }
}
