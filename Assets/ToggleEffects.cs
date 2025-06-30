using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class WinnerEffectsManager : MonoBehaviour
{
    public static WinnerEffectsManager Instance { get; private set; }

    [SerializeField] private VisualEffect fireWorksEffect;

    private TextMeshProUGUI winnerText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        winnerText = GetComponentInChildren<TextMeshProUGUI>();
        winnerText.alpha = 0f;
        //fireWorksEffect.Stop();
    }

    public void StartEffects(int playerID, Color color)
    {
        winnerText.alpha = 1f;
        winnerText.text = $"Player {playerID + 1} has won the game!";
        SetWinnerTextMaterial(color);
        //fireWorksEffect.Play();
    }

    public void StopEffects()
    {
        winnerText.alpha = 0f;
        fireWorksEffect.Stop();
    }

    public Material GetWinnerTextMaterial()
    {
        return GetComponentInChildren<MeshRenderer>().material;
    }

    public void SetWinnerTextMaterial(Color color)
    {
        GetComponentInChildren<TextMeshProUGUI>().color = color;
    }
}
