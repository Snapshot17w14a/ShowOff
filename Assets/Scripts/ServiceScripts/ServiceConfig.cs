using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Bootstrap/ServiceConfig")]
public class ServiceConfig : ScriptableObject
{
    [Header("PlayerRegistry settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string[] controlSchemes;
    [SerializeField] private Color[] playerColors;

    [Header("PlayerAutoJoin settings")]
    [SerializeField] private InputActionAsset inputActions;

    public void SetUpServices()
    {
        PlayerRegistry playerRegistry = new()
        {
            playerPrefab = playerPrefab,
            controlSchemes = controlSchemes,
            playerColors = playerColors
        };
        ServiceLocator.RegisterService(playerRegistry);

        PlayerAutoJoin playerAutoJoin = new()
        {
            inputActions = inputActions
        };
        ServiceLocator.RegisterService(playerAutoJoin);
    }
}
