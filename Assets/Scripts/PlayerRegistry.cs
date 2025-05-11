using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerRegistry : Service
{
    private RegisteredPlayer[] registeredPlayers;

    public GameObject playerPrefab;
    public string[] controlSchemes;

    private int players = 0;
    private int maxPlayers = 0;

    public int RegisteredPlayerCount => players;
    public int MaxPlayers => maxPlayers;

    public override void InitializeService()
    {
        maxPlayers = GameObject.FindFirstObjectByType<PlayerInputManager>().maxPlayerCount;
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
        var playerGameObject = CreatePlayer(playerToInstantiate.device, id);
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
        foreach (var player in registeredPlayers) InstantiatePlayerWithId(player.id);
    }

    private MinigamePlayer CreatePlayer(InputDevice device, int id)
    {
        return PlayerInput.Instantiate(playerPrefab, playerIndex: id, controlScheme: controlSchemes[Mathf.Min(players, maxPlayers)], pairWithDevice: device).GetComponent<MinigamePlayer>();
    }
}

public struct RegisteredPlayer
{
    public RegisteredPlayer(int id, InputDevice device, MinigamePlayer minigamePlayer = null)
    {
        this.id = id;
        this.device = device;
        this.minigamePlayer = minigamePlayer;
    }

    public int id;
    public InputDevice device;
    public MinigamePlayer minigamePlayer;
}
