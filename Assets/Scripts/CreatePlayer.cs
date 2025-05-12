using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPosition;
    PlayerInputManager playerInputManager;
    private int numPlayers = 0;
    private int maxPlayers = 0;

    [Space()]
    [Header("Events")]
    [SerializeField] private UnityEvent onPlayerJoin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        maxPlayers = playerInputManager.maxPlayerCount;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && numPlayers < maxPlayers)
        {
            ServiceLocator.GetService<PlayerRegistry>().CreatePlayerWithDevice(numPlayers < 2 ? Keyboard.current : Gamepad.current);
            numPlayers++;
        }
    }

    //Called by the PlayerInputManager component automatically
    private void OnPlayerJoined()
    {
        onPlayerJoin?.Invoke();
    }
    
    public void InstantiateAllPlayers()
    {
        ServiceLocator.GetService<PlayerRegistry>().InstantiateAllPlayers();
    }
}
