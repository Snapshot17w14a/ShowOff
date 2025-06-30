using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class WinnerEffectsManager : MonoBehaviour
{
    public static WinnerEffectsManager Instance { get; private set; }

    [SerializeField] private GameObject fireWorksObject;

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
    }

    public void StartEffects(int playerID, Color color, Vector3 position)
    {
        winnerText.alpha = 1f;
        winnerText.text = $"Player {playerID + 1} has won the game!";
        SetWinnerTextMaterial(color);
        GameObject vfx = Instantiate(fireWorksObject, position, Quaternion.identity);
        if (vfx.TryGetComponent<VisualEffect>(out var fireworkvfx))
        {
            fireworkvfx.SetVector4("Color Fireworks", color);
        }
    }

    public void StopEffects()
    {
        winnerText.alpha = 0f;
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
