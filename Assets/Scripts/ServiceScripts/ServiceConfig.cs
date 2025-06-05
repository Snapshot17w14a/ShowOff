using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Bootstrap/ServiceConfig")]
public class ServiceConfig : ScriptableObject
{
    [Header("PlayerRegistry settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Color[] playerColors;

    [Header("PlayerAutoJoin settings")]
    [SerializeField] private InputActionAsset inputActions;

    public void SetUpServices()
    {
        PlayerRegistry playerRegistry = new()
        {
            playerPrefab = playerPrefab,
            playerColors = playerColors
        };
        ServiceLocator.RegisterService(playerRegistry);

        PlayerAutoJoin playerAutoJoin = new()
        {
            inputActions = inputActions
        };
        ServiceLocator.RegisterService(playerAutoJoin);

        ScoreRegistry scoreRegistry = new();
        ServiceLocator.RegisterService(scoreRegistry);

        PauseManager pauseManager = new();
        ServiceLocator.RegisterService(pauseManager);

        new GameObject("Scheduler").AddComponent<Scheduler>();
    }
}
