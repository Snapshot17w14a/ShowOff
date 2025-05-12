using UnityEngine;

[CreateAssetMenu(menuName = "Bootstrap/ServiceConfig")]
public class ServiceConfig : ScriptableObject
{
    [Header("PlayerRegistry settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string[] controlSchemes;
    [SerializeField] private Color[] playerColors;

    public void SetUpServices()
    {
        PlayerRegistry playerRegistry = new()
        {
            playerPrefab = playerPrefab,
            controlSchemes = controlSchemes,
            playerColors = playerColors
        };
        ServiceLocator.RegisterService(playerRegistry);
    }
}
