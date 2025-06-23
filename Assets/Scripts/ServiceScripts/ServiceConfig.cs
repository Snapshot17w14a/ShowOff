using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Bootstrap/ServiceConfig")]
public class ServiceConfig : ScriptableObject
{
    [Header("PlayerRegistry settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PlayerVisualData[] playerVisualData;
    [SerializeField] GameObject audioManager;

    [Header("PlayerAutoJoin settings")]
    [SerializeField] private InputActionAsset inputActions;

    public void SetUpServices()
    {
        PlayerRegistry playerRegistry = new()
        {
            playerPrefab = playerPrefab,
            visualData = playerVisualData
        };
        Services.RegisterService(playerRegistry);

        PlayerAutoJoin playerAutoJoin = new()
        {
            inputActions = inputActions
        };
        Services.RegisterService(playerAutoJoin);

        ScoreRegistry scoreRegistry = new();
        Services.RegisterService(scoreRegistry);

        PauseManager pauseManager = new();
        Services.RegisterService(pauseManager);

        new GameObject("Scheduler").AddComponent<Scheduler>();
        Instantiate(audioManager);
    }
}
