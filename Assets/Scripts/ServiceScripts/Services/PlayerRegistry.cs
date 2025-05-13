using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class PlayerRegistry : Service
{
    public GameObject playerPrefab;
    public string[] controlSchemes;
    public Color[] playerColors;

    private readonly List<InputDevice> usedInputDevices = new();
    private RegisteredPlayer[] registeredPlayers;

    private int players = 0;
    private int maxPlayers = 0;

    private int keyboardPlayers = 0;

    public int RegisteredPlayerCount => players;
    public int MaxPlayers => maxPlayers;

    public event Action<MinigamePlayer> OnPlayerSpawn;

    public override void InitializeService()
    {
        var playerInputManager = GameObject.FindFirstObjectByType<PlayerInputManager>();

        maxPlayers = playerInputManager == null ? 8 : playerInputManager.maxPlayerCount; 
        registeredPlayers = new RegisteredPlayer[maxPlayers];
    }

    /// <summary>
    /// Create a player and pair it with an input device, optionally instantiate it immediatly
    /// </summary>
    /// <param name="device"><see cref="InputDevice"/> that the player will be paired with</param>
    /// <returns>The created user <see cref="MinigamePlayer"/></returns>
    public MinigamePlayer CreatePlayerWithDevice(InputDevice device, bool instantiatePlayer = true)
    {
        //Create a RegisteredPlayer struct to hold the player's device, id and the reference to its GameObject if it exists
        var regPlayer = new RegisteredPlayer()
        {
            device = device,
            id = players,
            minigamePlayer = instantiatePlayer ? CreatePlayer(device, players) : null
        };

        //Add it to the array and increment the number of players
        registeredPlayers[regPlayer.id] = regPlayer;
        players++;

        return regPlayer.minigamePlayer;
    }

    /// <summary>
    /// Instantiate a player with the given id
    /// </summary>
    public MinigamePlayer InstantiatePlayerWithId(int id)
    {
        //Grab a copy of the player with the id
        var playerToInstantiate = registeredPlayers[id];

        //If the player already has a GameObject in the scene log an error
        if (playerToInstantiate.minigamePlayer != null)
        {
            Debug.LogError("Player is already instantiated");
            return null;
        }

        //Instantiate the player and store it's reference in the RegisteredPlayer struct
        var playerGameObject = CreatePlayer(playerToInstantiate/*playerToInstantiate.device, id*/);
        playerToInstantiate.minigamePlayer = playerGameObject;

        //Reassign the struct with the struct that now stores the GameObject reference
        registeredPlayers[id] = playerToInstantiate;

        return playerToInstantiate.minigamePlayer;
    }

    /// <summary>
    /// Instantiate all players stored in the registry, useful when loading a different scene
    /// </summary>
    public void InstantiateAllPlayers()
    {
        foreach (var player in registeredPlayers) if (!RegisteredPlayer.IsNull(player)) InstantiatePlayerWithId(player.id);
    }

    /// <summary>
    /// Check wether a device is already in use by another player
    /// </summary>
    /// <returns>True if the device is already in use</returns>
    public bool DoesPlayerWithDeviceExist(InputDevice device)
    {
        if (device == Keyboard.current && keyboardPlayers < 2) return false; 
        return usedInputDevices.Contains(device);
    }

    private MinigamePlayer CreatePlayer(InputDevice device, int id)
    {
        //Instantiate the player with the given device, id and choose a free control scheme
        var player = PlayerInput.Instantiate(playerPrefab, playerIndex: id, controlScheme: ControlSchemeForDevice(device), pairWithDevice: device).GetComponent<MinigamePlayer>();

        //Set the color of the player indicators
        player.SetPlayerColor(playerColors[id], id);

        //Add the paired device to the list of already in use devices
        usedInputDevices.Add(device);

        //Invoke the event with the created player
        OnPlayerSpawn?.Invoke(player);

        return player;
    }

    private MinigamePlayer CreatePlayer(RegisteredPlayer registeredPlayer)
    {
        //Instantiate the player with the given device, id and choose a free control scheme
        var player = PlayerInput.Instantiate(playerPrefab, playerIndex: registeredPlayer.id, controlScheme: registeredPlayer.controlScheme, pairWithDevice: registeredPlayer.device).GetComponent<MinigamePlayer>();

        //Set the color of the player indicators
        player.SetPlayerColor(playerColors[registeredPlayer.id], registeredPlayer.id);

        //Invoke the event with the created player
        OnPlayerSpawn?.Invoke(player);

        return player;
    }

    private string ControlSchemeForDevice(InputDevice device)
    {
        if (device is Gamepad) return "Controller";
        if (device is Keyboard) return ++keyboardPlayers == 1 ? "KeyboardLeft" : "KeyboardRight";
        return "";
    }
}

public struct RegisteredPlayer
{
    public RegisteredPlayer(int id, InputDevice device, MinigamePlayer minigamePlayer = null, string controlScheme = "")
    {
        this.id = id;
        this.controlScheme = controlScheme;
        this.device = device;
        this.minigamePlayer = minigamePlayer;
    }

    public int id;
    public string controlScheme;
    public InputDevice device;
    public MinigamePlayer minigamePlayer;

    public static bool IsNull(RegisteredPlayer registeredPlayer)
    {
        return registeredPlayer.id == 0 && registeredPlayer.device == null && registeredPlayer.minigamePlayer == null;
    }
}
