using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Podium : MonoBehaviour
{
    [HideInInspector] public MinigamePlayer player;

    [SerializeField] private float maxHeight;

    public static int highestScore = 0;
    public static PodiumController controller;

    private TextMeshPro scoreText;
    private int podiumScoreLimit;

    private Vector3 startingPosition;
    private Vector3 targetPosition;

    private Vector3 startingScale;
    private Vector3 targetScale;

    private Vector3 startingPlayerPosition;
    private Vector3 targetPlayerPosition;

    private Vector3 startingTextPosition;
    private Vector3 targetTextPosition;

    private float timer;

    public void Initialize()
    {
        podiumScoreLimit = ServiceLocator.GetService<ScoreRegistry>().ScoreOfPlayer(player);
        player.transform.rotation = Quaternion.Euler(0, 180f, 0);
        SetPlayerInteraction(false);

        GetComponent<MeshRenderer>().material.color = player.GetComponent<MeshRenderer>().material.color;

        scoreText = controller.CreateScoreText().GetComponent<TextMeshPro>();

        scoreText.transform.position = transform.position + new Vector3(0, 0, -0.6f);
        player.transform.position = transform.position;
    }

    private void Update()
    {
        timer += Mathf.Clamp01(Time.deltaTime * controller.ScorePerSecond);

        transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, timer);
        transform.localScale = Vector3.Lerp(startingScale, targetScale, timer);
        player.transform.position = Vector3.Lerp(startingPlayerPosition, targetPlayerPosition, timer);
        scoreText.transform.position = Vector3.Lerp(startingTextPosition, targetTextPosition, timer);

    }

    public void UpdateScaleAndPosition(int score)
    {
        if (score > podiumScoreLimit) return;

        timer = 0;

        scoreText.text = score.ToString();

        var height = maxHeight * (score / (float)highestScore);
        height = score == 0 ? 0 : height;

        startingScale = transform.localScale;
        targetScale = new Vector3(1, height, 1);

        var localPos = transform.localPosition;

        startingPosition = localPos;
        targetPosition = new Vector3(localPos.x, targetScale.y / 2f, localPos.z);

        startingTextPosition = scoreText.transform.position;
        targetTextPosition = transform.position + new Vector3(0, targetScale.y / 2f, -0.6f);

        startingPlayerPosition = player.transform.position;
        targetPlayerPosition = transform.position + new Vector3(0, (targetScale.y / 2f) + 0.25f, 0);
    }

    public void SetPlayerInteraction(bool state)
    {
        player.GetComponent<Rigidbody>().isKinematic = !state;
        var input = player.GetComponent<PlayerInput>();

        if (state) input.ActivateInput();
        else input.DeactivateInput();
    }

    public void CleanUp()
    {
        //Destroy(scoreText.gameObject);
    }
}
