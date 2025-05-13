using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerAutoJoin : Service
{
    public InputActionAsset inputActions;

    /// <summary>
    /// If set true any new devices that use any of the ingame bindings will create a new player
    /// </summary>
    public bool AutoJoinPlayers { get; set; } = true;

    /// <summary>
    /// Controls wether creating new players is allowed or not
    /// </summary>
    public bool AllowJoining
    {
        get => joinAction.enabled;
        set 
        {
            if (value) joinAction.Enable();
            else joinAction.Disable();
        }
    }

    /// <summary>
    /// Fired when a new player joins with a new device
    /// </summary>
    public event Action<MinigamePlayer> OnPlayerJoin;

    private InputAction joinAction;

    public override void InitializeService()
    {
        //Create a list to store all binding used to interact with the players
        List<string> bindings = new();

        //Extract all the bindings in the Player action map from the inputActions asset
        foreach (var binding in inputActions.FindActionMap("Player").bindings)
        {
            string currentBindingString = binding.effectivePath;

            //Some of the bindings are just orgaizers, meaning they are "Vector2D" and not actual binding, we skip these
            if (currentBindingString[0] != '<') continue;

            //If the binding is not skipped add it to the list of bindings
            bindings.Add(currentBindingString);
        }

        //Create an InputAction that will hold the binding to listen for
        joinAction = new(
            name: "JoinAction",
            binding: "",
            interactions: "",
            processors: ""
        );

        //Add each binding to the joinAction
        foreach (var binding in bindings) joinAction.AddBinding(binding);

        //Add the method to be called each time any connected device performs an action with any relevant binding
        joinAction.performed += context => CreatePlayerIfPossible(context.control.device);

        //Enable the action to listen for inputs
        AllowJoining = true;
    }

    //Check wether the device performing the action is already assigned with a player, if not create a new player,
    private void CreatePlayerIfPossible(InputDevice device)
    {
        //Get a reference to the player registry
        var playerRegistry = ServiceLocator.GetService<PlayerRegistry>();

        //If a player with the given device already exists ignore it
        if (playerRegistry.DoesPlayerWithDeviceExist(device)) return;

        //Create the player with the device and fire an event using the MinigamePlayer reference returned from the registry
        var player = playerRegistry.CreatePlayerWithDevice(device);
        OnPlayerJoin?.Invoke(player);
    }
}
