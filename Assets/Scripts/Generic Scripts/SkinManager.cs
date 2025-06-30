using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class SkinManager : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] playerMeshRenderers;
    [SerializeField] private SpriteRenderer[] spritesToRecolor;
    [SerializeField] private GameObject dashIndicator;
    [SerializeField] private PlayerVisualData goldVisual;
    [SerializeField] private Material goldDashMaterial;
    [SerializeField] private GameObject crownParent;
    [SerializeField] private GameObject crown;

    public Color playerColor;
    public int RegistryID => GetComponent<MinigamePlayer>().RegistryID; 

    public void ChangeSkin()
    {
        RegisteredPlayer data = Services.Get<PlayerRegistry>().GetPlayerData(RegistryID);


        if (data.isLastWinner)
        {
            var winnerEffectsManager = WinnerEffectsManager.Instance;
            if (winnerEffectsManager != null)
                winnerEffectsManager.StartEffects(data.id, playerColor, transform.position);

            SetPlayerColor(goldVisual, RegistryID);
            dashIndicator.GetComponent<MeshRenderer>().material = goldDashMaterial;
            GetComponent<MinigamePlayer>().SetDashIndicatorMaterial(dashIndicator.GetComponent<MeshRenderer>().material);
            EnableCrown(true);
        }
    }

    private void EnableCrown(bool state)
    {
        crown.SetActive(state);
    }

    public void SetPlayerColor(PlayerVisualData data, int playerId)
    {
        foreach (var renderer in spritesToRecolor) renderer.color = new Color(data.color.r, data.color.g, data.color.b, renderer.color.a);
        EnableCrown(false);
        ForEachPlayerRenderer(r => r.material = data.material);

        var textMeshPro = GetComponentInChildren<TextMeshPro>();
        textMeshPro.color = data.color;
        textMeshPro.text = $"P{playerId + 1}";

        dashIndicator.GetComponent<MeshRenderer>().material.SetColor("_ColorCircle", data.color);

        SetDualshockColor(data, playerId);

        playerColor = data.color;
    }

    private void SetDualshockColor(PlayerVisualData data, int playerId)
    {
        InputDevice inputDevie = GetComponent<PlayerInput>().devices[0];

        if (inputDevie is DualShockGamepad dualshock)
        {
            dualshock.SetLightBarColor(data.color);
        }
    }

    private void ForEachPlayerRenderer(Action<SkinnedMeshRenderer> function)
    {
        foreach (var renderer in playerMeshRenderers) function(renderer);
    }
}
