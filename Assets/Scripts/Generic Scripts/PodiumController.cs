using UnityEngine;
using UnityEngine.Events;

public class PodiumController : MonoBehaviour
{
    [SerializeField] private GameObject podiumPrefab;
    [SerializeField] private GameObject scoreTextPrefab;
    [SerializeField] private Transform scoreTextParent;

    [SerializeField] private float spacing;
    [SerializeField] private int scorePerSecond;

    [SerializeField] private HubScoreState scoreState;

    public int ScorePerSecond => scorePerSecond;
    public int CurrentScore => currentScore;
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
        var playerRegistry = ServiceLocator.GetService<PlayerRegistry>();
        int playerCount = playerRegistry.RegisteredPlayerCount;

        //If there are no players registeresd 
        if (playerCount == 0)
        {
            scoreState.SkipPodiumStage();
            return;
        }

        //Get the highest score from the ScoreRegistry
        var scoreRegistry = ServiceLocator.GetService<ScoreRegistry>();
        PlayerScore highestScore = scoreRegistry.HighestScore;
        Podium.highestScore = highestScore.score;
        Podium.controller = this;

        //If there is a unique winner (no tie) increment the win streak, set all others' wind flag to false
        if (highestScore.isUnique)
        {
            playerRegistry.ExecuteForEachPlayerData(data =>
            {
                if (data.id == highestScore.id)
                {
                    winnerID = highestScore.id;
                    data.winStreak++;
                }
                else data.isLastWinner = false;
                return data;
            });
        }
        //If there are no unique winners reset all winner flags to false, and set the win streak back to 0
        else
        {
            
            playerRegistry.ExecuteForEachPlayerData(data =>
            {
                data.isLastWinner = false;
                data.winStreak = 0;
                return data;
            });
        }

        //Set the winner's data to have winner as true
        if (highestScore.isUnique)
        {
            var winnerData = playerRegistry.GetPlayerData(highestScore.id);
            winnerData.isLastWinner = true;
            winnerID = highestScore.id;
            winnerData.winStreak++;
            playerRegistry.SetPlayerData(winnerData);
        }

        podiums = new Podium[playerCount];

        //Create a podium for all players
        for (int i = 0; i < playerCount; i++)
        {
            podiums[i] = CreatePodium(i, playerRegistry);
        }

        //Set the position to center all the podiums
        var parentPos = transform.position;
        parentPos.x = -((playerCount + spacing * Mathf.Max(playerCount - 1, 0)) / 2f);
        transform.position = parentPos;

        //Initialize the podiums, if all players get a score of 0 set the progress limit to 1 to avoid dividing by 0
        for (int i = 0; i < podiums.Length; i++)
        {
            if (highestScore.score == 0)
                podiums[i].Initialize(1);
            else
                podiums[i].Initialize(scoreRegistry.ScoreOfPlayer(i) / (float)highestScore.score);
        }

        //Set up the Animation and start it
        targetScore = highestScore.score;
        currentScore = 0;
        OnCountStart?.Invoke();

        //Start the lerping with the scheduler
        Scheduler.Instance.Lerp(UpdatePodiums, targetScore * CooldownPerScore, PostAnimation);
    }

    //Create a podium, set its position based on the index
    private Podium CreatePodium(int index, PlayerRegistry playerRegistry)
    {
        var pos = new Vector3(0.5f * (index + 1) + (0.5f + spacing) * index, -0.3f, 0);
        var podium = Instantiate(podiumPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Podium>();
        podium.transform.localPosition = pos;
        podium.transform.localScale = new Vector3(1, 0, 1);
        podium.player = playerRegistry.InstantiatePlayerWithId(index);

        return podium;
    }
    
    private void UpdatePodiums(float t)
    {
        currentScore = Mathf.RoundToInt(targetScore * t);
        foreach (var podium in podiums) podium.UpdateLerp(t);
    }

    private void PostAnimation()
    {
        CleanUp();

        if (winnerID != -1)
            ServiceLocator.GetService<PlayerRegistry>().GetPlayerData(winnerID).minigamePlayer.GetComponent<SkinManager>().ChangeSkin();

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
