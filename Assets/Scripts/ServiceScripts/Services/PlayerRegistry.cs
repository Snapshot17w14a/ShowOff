using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRegistry : Service
{
    public GameObject playerPrefab;
    public PlayerVisualData[] visualData;

    private readonly List<InputDevice> usedInputDevices = new();
    private RegisteredPlayer[] registeredPlayers;

    private Stack<int> availableIDs;

    private int players = 0;
    private int maxPlayers = 0;
    private int keyboardPlayers = 0;

    public int RegisteredPlayerCount => players;
    public int MaxPlayers => maxPlayers;
    public Span<RegisteredPlayer> AllPlayers => registeredPlayers;

    /// <summary>
    /// When a player is instantiated, this event is triggered and a reference to the instantiated <see cref="MinigamePlayer"/> is passed as a parameter
    /// </summary>
    public event Action<MinigamePlayer> OnPlayerSpawn;

    /// <summary>
    /// Triggered when a player is registered, and stored into the registered players array
    /// </summary>
    public event Action<int> OnPlayerRegistered;

    /// <summary>
    /// After a player is disconnected this event is triggered and the id of the disconnected player is passed as a parameter
    /// </summary>
    public event Action<int> OnAfterPlayerDisconnect;

    /// <summary>
    /// Called before the disconnecting player's Player is destroyed, useful for cleaning up
    /// </summary>
    public event Action<MinigamePlayer> OnBeforePlayerDisconnect;

    public override void InitializeService()
    {
        var playerInputManager = GameObject.FindFirstObjectByType<PlayerInputManager>();

        maxPlayers = playerInputManager == null ? 8 : playerInputManager.maxPlayerCount;
        availableIDs = new(maxPlayers);
        registeredPlayers = new RegisteredPlayer[maxPlayers];

        for (int i = maxPlayers - 1; i >= 0; i--) availableIDs.Push(i);
    }

    /// <summary>
    /// Create a player and pair it with an input device, optionally instantiate it immediatly
    /// </summary>
    /// <param name="device"><see cref="InputDevice"/> that the player will be paired with</param>
    /// <returns>The created user <see cref="MinigamePlayer"/></returns>
    public MinigamePlayer CreatePlayerWithDevice(InputDevice device, bool instantiatePlayer = true, string controlScheme = "")
    {
        if (controlScheme == "") controlScheme = ControlSchemeForDevice(device);

        //Get the next available id
        var id = availableIDs.Pop();

        //Create a RegisteredPlayer struct to hold the player's device, id and the reference to its GameObject if it exists
        var regPlayer = new RegisteredPlayer()
        {
            device = device,
            id = id,
            minigamePlayer = instantiatePlayer ? CreatePlayer(device, id, controlScheme) : null,
            controlScheme = controlScheme
        };

        //Add it to the array and increment the number of players
        registeredPlayers[id] = regPlayer;
        players++;

        //Trigger the registered event
        OnPlayerRegistered?.Invoke(id);

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
            Debug.LogWarning("Player is already instantiated");
            return null;
        }

        //Instantiate the player and store it's reference in the RegisteredPlayer struct
        var playerGameObject = CreatePlayer(playerToInstantiate);
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

    /// <summary>
    /// Pass in a fuction to execute of all players
    /// </summary>
    public void ExecuteForEachPlayer(Action<MinigamePlayer> function)
    {
        foreach (var player in registeredPlayers)
        {
            if (RegisteredPlayer.IsNull(player)) continue;
            function(player.minigamePlayer);
        }
    }

    /// <summary>
    /// Pass in a fucntion modifying and returning a <see cref="RegisteredPlayer"/>
    /// </summary>
    public void ExecuteForEachPlayerData(Func<RegisteredPlayer, RegisteredPlayer> function)
    {
        for (int i = 0; i < registeredPlayers.Length; i++)
        {
            //if (RegisteredPlayer.IsNull(registeredPlayers[i])) continue;
            registeredPlayers[i] = function(registeredPlayers[i]);
        }
    }

    private MinigamePlayer CreatePlayer(InputDevice device, int id, string controlScheme = "")
    {
        //Instantiate the player with the given device, id and choose a free control scheme
        var player = PlayerInput.Instantiate(playerPrefab, playerIndex: id, controlScheme: controlScheme, pairWithDevice: device).GetComponent<MinigamePlayer>();
        SkinManager skinManager = player.GetComponent<SkinManager>();

        player.GetComponent<PlayerInput>().neverAutoSwitchControlSchemes = true;

        //Set the color of the player indicators
        skinManager.SetPlayerColor(visualData[id], id);

        //Set the id reference in the minigame player
        player.RegistryID = id;

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
        SkinManager skinManager = player.GetComponent<SkinManager>();

        player.GetComponent<PlayerInput>().neverAutoSwitchControlSchemes = true;

        //Set the color of the player indicators
        skinManager.SetPlayerColor(visualData[registeredPlayer.id], registeredPlayer.id);

        //Set the id reference in the minigame player
        player.RegistryID = registeredPlayer.id;

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

    /// <summary>
    /// Disconnect a user, and free up the device associated with it
    /// </summary>
    /// <param name="id">ID of the user in the registry</param>
    public void DisconnectUser(int id)
    {
        Debug.Log("Disconnecting user with id: " + id);

        //Get a copy of the player data
        var regPlayer = registeredPlayers[id];

        //Call the event for anything that needs the player's reference before disconnecting
        OnBeforePlayerDisconnect?.Invoke(regPlayer.minigamePlayer);

        //If the player has a gameobject in the scene destroy it
        if (regPlayer.minigamePlayer != null) GameObject.Destroy(regPlayer.minigamePlayer.gameObject);

        //Remove the device from the list of used devices to allow it to join again
        usedInputDevices.Remove(regPlayer.device);

        //Decrement the keyboardPlayers amount and add back the bindings to the auto join to allow them to join again
        if (regPlayer.device == Keyboard.current)
        {
            keyboardPlayers--;
            Services.Get<PlayerAutoJoin>().AddBackKeyboardBindings(regPlayer.controlScheme);
        }

        //Push the id of the disconnected player to allow it to he used again
        availableIDs.Push(id);

        //Decrement the player amount
        players--;

        //Reset the values so IsNull returns true
        regPlayer.id = 0;
        regPlayer.device = null;

        //Reassign the data to the registry;
        registeredPlayers[id] = regPlayer;

        //Trigger the disconnect event with the id of the disconnected user
        OnAfterPlayerDisconnect?.Invoke(id);
    }

    public int IdOf(MinigamePlayer player) => registeredPlayers.Where(regPlayer => regPlayer.minigamePlayer.Equals(player)).First().id;

    /// <summary>
    /// Get a copy of the stored player data in the form of a RegisteredPlayer struct
    /// </summary>
    /// <param name="id">ID of the player, stored in the <see cref="MinigamePlayer"/> as <see cref="MinigamePlayer.RegistryID"/></param>
    public RegisteredPlayer GetPlayerData(int id) => registeredPlayers[id];

    /// <summary>
    /// Set the registry's data with a custom <see cref="RegisteredPlayer"/> struct. Use with caution, can easily break things if wrong parameters are used. Use <see cref="PlayerRegistry.GetPlayerData(int)"/> first, change values and then reassign using <see cref="PlayerRegistry.SetPlayerData(RegisteredPlayer)"/>.
    /// </summary>
    /// <param name="playerData">The struct holding the data</param>
    public void SetPlayerData(RegisteredPlayer playerData)
    {
        registeredPlayers[playerData.id] = playerData;
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
        isLastWinner = false;
        winStreak = 0;
    }

    public int id;
    public string controlScheme;
    public InputDevice device;
    public MinigamePlayer minigamePlayer;

    //Stats
    public bool isLastWinner;
    public int winStreak;

    public static bool IsNull(RegisteredPlayer registeredPlayer)
    {
        return registeredPlayer.id == 0 && registeredPlayer.device == null && registeredPlayer.minigamePlayer == null;
    }
}
