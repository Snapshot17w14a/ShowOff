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

    public void Initialize()
    {
        podiumScoreLimit = ServiceLocator.GetService<ScoreRegistry>().ScoreOfPlayer(player);
        player.transform.rotation = Quaternion.Euler(0, 180f, 0);
        SetPlayerInteraction(false);

        GetComponent<MeshRenderer>().material.color = player.GetComponent<MeshRenderer>().material.color;

        scoreText = controller.CreateScoreText().GetComponent<TextMeshPro>();
    }

    public void UpdateScaleAndPosition(int score)
    {
        if (score > podiumScoreLimit) return;

        scoreText.text = score.ToString();

        var height = Mathf.Lerp(0, maxHeight, score / (float)highestScore);
        height = score == 0 ? 0 : height;
        transform.localScale = new Vector3(1, height, 1);

        var localPos = transform.localPosition;
        transform.localPosition = new Vector3(localPos.x, transform.localScale.y / 2f, localPos.z);

        scoreText.transform.position = transform.position + new Vector3(0, transform.localScale.y / 2f - 0.2f, -0.6f);

        player.transform.position = transform.position + new Vector3(0, (transform.localScale.y / 2f) + 0.6f, 0);
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
        Destroy(scoreText.gameObject);
    }
}
