using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CreatePlayer : MonoBehaviour
{
    public event Action<MinigamePlayer> OnPlayerSpawn;

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

    private Gamepad[] gamepads;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        maxPlayers = playerInputManager.maxPlayerCount;
        gamepads = Gamepad.all.ToArray();

        //inputActionAsset.FindActionMap("Player").actionTriggered += context => { Debug.Log(context); };

        //foreach (var binding in inputActionAsset.FindActionMap("Player").bindings)
        //{
        //    Debug.Log(binding.effectivePath);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && numPlayers < maxPlayers)
        {
            MinigamePlayer player = Services.Get<PlayerRegistry>().CreatePlayerWithDevice(numPlayers < 2 ? Keyboard.current : gamepads[numPlayers - 2]);
            numPlayers++;
            OnPlayerSpawn?.Invoke(player);
        }
    }

    //Called by the PlayerInputManager component automatically
    private void OnPlayerJoined()
    {
        onPlayerJoin?.Invoke();
    }

    public void InstantiateAllPlayers()
    {
        Services.Get<PlayerRegistry>().InstantiateAllPlayers();
    }
}
