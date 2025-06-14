using System;
using TMPro;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] playerMeshRenderers;
    [SerializeField] private SpriteRenderer[] spritesToRecolor;
    [SerializeField] private GameObject dashIndicator;
    [SerializeField] private PlayerVisualData goldVisual;
    [SerializeField] private Material goldDashMaterial;
    [SerializeField] private GameObject crown;

    private Material dashIndicatorMaterial;

    public Color playerColor;
    public int RegistryID { get; set; }

    private void Awake()
    {
        dashIndicatorMaterial = dashIndicator.GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        ServiceLocator.GetService<ScoreRegistry>().AddPlayer(RegistryID);
    }

    public void ChangeSkin()
    {
        RegisteredPlayer data = ServiceLocator.GetService<PlayerRegistry>().GetPlayerData(RegistryID);

        if (data.isLastWinner)
        {
            SetPlayerColor(goldVisual, RegistryID);
            dashIndicator.GetComponent<MeshRenderer>().material = goldDashMaterial;
            dashIndicatorMaterial = dashIndicator.GetComponent<MeshRenderer>().material;
            EnableCrown();
        }
    }

    private void EnableCrown()
    {

    }

    public void SetPlayerColor(PlayerVisualData data, int playerId)
    {
        foreach (var renderer in spritesToRecolor) renderer.color = new Color(data.color.r, data.color.g, data.color.b, renderer.color.a);

        ForEachPlayerRenderer(r => r.material = data.material);

        var textMeshPro = GetComponentInChildren<TextMeshPro>();
        textMeshPro.color = data.color;
        textMeshPro.text = $"P{playerId + 1}";

        dashIndicator.GetComponent<MeshRenderer>().material.SetColor("_ColorCircle", data.color);

        playerColor = data.color;
    }

    private void ForEachPlayerRenderer(Action<SkinnedMeshRenderer> function)
    {
        foreach (var renderer in playerMeshRenderers) function(renderer);
    }
}
