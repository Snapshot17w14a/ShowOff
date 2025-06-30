using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Podium : MonoBehaviour
{
    [HideInInspector] public MinigamePlayer player;

    [SerializeField] private MeshRenderer podiumTopRenderer;
    [SerializeField] private float maxHeight;

    public static int highestScore = 0;
    public static PodiumController controller;

    private TextMeshPro scoreText;

    private Vector3 startingScale;
    private Vector3 targetScale;

    private Vector3 startingPlayerPosition;
    private Vector3 targetPlayerPosition;

    private Vector3 startingTextPosition;
    private Vector3 targetTextPosition;

    private float maxLerpTime;

    public void Initialize(float progressLimitMult)
    {
        player.transform.rotation = Quaternion.Euler(0, 180f, 0);
        SetPlayerInteraction(false);

        var playerColor = player.GetComponent<SkinManager>().playerColor;
        GetComponent<MeshRenderer>().material.SetColor("_Base_color", playerColor);
        podiumTopRenderer.material.SetColor("_Base_color", playerColor);

        scoreText = controller.CreateScoreText().GetComponent<TextMeshPro>();

        var position = transform.position;

        startingScale = transform.localScale;
        targetScale = new Vector3(1, maxHeight, 1);

        startingTextPosition = position + new Vector3(0, 0, -0.6f);
        targetTextPosition = position + new Vector3(0, targetScale.y / 2f, -0.6f);

        startingPlayerPosition = position + new Vector3(0, .15f, 0);
        targetPlayerPosition = position + new Vector3(0, targetScale.y + .15f, 0);

        player.transform.position = startingPlayerPosition;

        maxLerpTime = progressLimitMult;
    }

    public void UpdateLerp(float t)
    {
        if (t > maxLerpTime) return;

        scoreText.text = controller.CurrentScore.ToString();

        transform.localScale = Vector3.Lerp(startingScale, targetScale, t);
        player.transform.position = Vector3.Lerp(startingPlayerPosition, targetPlayerPosition, t);
        scoreText.transform.position = Vector3.Lerp(startingTextPosition, targetTextPosition, t);
    }

    public void SetPlayerInteraction(bool state)
    {
        player.GetComponent<Rigidbody>().isKinematic = !state;
        var input = player.GetComponent<PlayerInput>();

        if (state) input.ActivateInput();
        else input.DeactivateInput();
    }

    public void AddRenderMaterialsToDissolve(HubScoreState state)
    {
        state.AddMaterial(GetComponent<MeshRenderer>().material);
        state.AddMaterial(podiumTopRenderer.material);
    }
}
