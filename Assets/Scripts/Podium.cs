using UnityEngine;

public class Podium : MonoBehaviour
{
    [HideInInspector] public MinigamePlayer player;

    private int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = ServiceLocator.GetService<ScoreRegistry>().ScoreOfPlayer(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
