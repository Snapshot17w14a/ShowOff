using UnityEngine;
using TMPro;

public class Podium : MonoBehaviour
{
    [HideInInspector] public MinigamePlayer player;

    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private float maxHeight;

    public static int HighestScore = 0;

    private int podiumScoreLimit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        podiumScoreLimit = ServiceLocator.GetService<ScoreRegistry>().ScoreOfPlayer(player);
    }

    public void UpdateScaleAndPosition(int score)
    {
        if (score > podiumScoreLimit) return;

        scoreText.text = score.ToString();

        var height = Mathf.Lerp(0, maxHeight, score / (float)HighestScore);
        height = score == 0 ? 0 : height;
        transform.localScale = new Vector3(1, height, 1);

        var localPos = transform.localPosition;
        transform.localPosition = new Vector3(localPos.x, transform.localScale.y / 2f, localPos.z);
    }
}
