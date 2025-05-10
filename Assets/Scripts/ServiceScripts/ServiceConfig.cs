using UnityEngine;

[CreateAssetMenu(menuName = "Bootstrap/ServiceConfig")]
public class ServiceConfig : ScriptableObject
{
    [Header("PlayerRegistry settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string[] controlSchemes;

    public void SetUpServices()
    {
        PlayerRegistry playerRegistry = new()
        {
            playerPrefab = playerPrefab,
            controlSchemes = controlSchemes
        };
        ServiceLocator.RegisterService(playerRegistry);
    }
}
