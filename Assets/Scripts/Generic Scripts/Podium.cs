using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Podium : MonoBehaviour
{
    [HideInInspector] public MinigamePlayer player;

    [SerializeField] private float maxHeight;

    public static int highestScore = 0;
    public static PodiumController controller;

    private TextMeshPro scoreText;

    private Vector3 startingPosition;
    private Vector3 targetPosition;

    private Vector3 startingScale;
    private Vector3 targetScale;

    private Vector3 startingPlayerPosition;
    private Vector3 targetPlayerPosition;

    private Vector3 startingTextPosition;
    private Vector3 targetTextPosition;

    public void Initialize(float progressLimitMult)
    {
        player.transform.rotation = Quaternion.Euler(0, 180f, 0);
        SetPlayerInteraction(false);

        GetComponent<MeshRenderer>().material.color = player.GetComponent<MeshRenderer>().material.color;

        scoreText = controller.CreateScoreText().GetComponent<TextMeshPro>();

        var position = transform.position;

        scoreText.transform.position = position + new Vector3(0, 0, -0.6f);
        player.transform.position = position;

        startingScale = transform.localScale;
        targetScale = new Vector3(1, maxHeight * progressLimitMult, 1);

        startingPosition = transform.localPosition;
        targetPosition = new Vector3(startingPosition.x, targetScale.y / 2f, startingPosition.z);

        startingTextPosition = position;
        targetTextPosition = position + new Vector3(0, targetScale.y / 2f, -0.6f);

        startingPlayerPosition = position;
        targetPlayerPosition = position + new Vector3(0, (targetScale.y / 2f) + 0.25f, 0);
    }

    //private void Update()
    //{
    //    timer += Mathf.Clamp01(Time.deltaTime * controller.ScorePerSecond);

    //    transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, timer);
    //    transform.localScale = Vector3.Lerp(startingScale, targetScale, timer);
    //    player.transform.position = Vector3.Lerp(startingPlayerPosition, targetPlayerPosition, timer);
    //    scoreText.transform.position = Vector3.Lerp(startingTextPosition, targetTextPosition, timer);

    //}

    public void UpdateLerp(float t)
    {
        transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, t);
        transform.localScale = Vector3.Lerp(startingScale, targetScale, t);
        player.transform.position = Vector3.Lerp(startingPlayerPosition, targetPlayerPosition, t);
        scoreText.transform.position = Vector3.Lerp(startingTextPosition, targetTextPosition, t);
    }

    //public void UpdateScaleAndPosition(int score)
    //{
    //    if (score > podiumScoreLimit) return;

    //    timer = 0;

    //    scoreText.text = score.ToString();

    //    var height = maxHeight * (score / (float)highestScore);
    //    height = score == 0 ? 0 : height;

    //    startingScale = transform.localScale;
    //    targetScale = new Vector3(1, height, 1);

    //    var localPos = transform.localPosition;

    //    startingPosition = localPos;
    //    targetPosition = new Vector3(localPos.x, targetScale.y / 2f, localPos.z);

    //    startingTextPosition = scoreText.transform.position;
    //    targetTextPosition = transform.position + new Vector3(0, targetScale.y / 2f, -0.6f);

    //    startingPlayerPosition = player.transform.position;
    //    targetPlayerPosition = transform.position + new Vector3(0, (targetScale.y / 2f) + 0.25f, 0);
    //}

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
