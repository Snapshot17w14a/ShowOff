using UnityEngine;
using UnityEngine.InputSystem;

public class CreatePlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    PlayerInputManager playerInputManager;
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private Transform spawnPosition;
    private int numPlayers = 0;
    private int maxPlayers = 0;

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
            PlayerInput.Instantiate(playerPrefab, controlScheme: numPlayers == 0 ? "KeyboardLeft" : "KeyboardRight", pairWithDevice: Keyboard.current);
            numPlayers++;
        }
    }
}
