using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

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

    private readonly Dictionary<string, InputBinding[]> removedKeyboardBindings = new(2);

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

        //Create an InputAction that will hold the bindings to listen for
        joinAction = new(
            name: "JoinAction",
            binding: "",
            interactions: "",
            processors: ""
        );

        //Add each binding to the joinAction
        foreach (var binding in bindings) joinAction.AddBinding(binding);

        //Add the method to be called each time any connected device performs an action with any relevant binding
        joinAction.performed += context => CreatePlayerIfPossible(context);

        //Enable the action to listen for inputs
        AllowJoining = true;
    }

    /// <summary>
    /// Add the bindings back to the join action so that a disconnected keyboard user can join again
    /// </summary>
    /// <param name="bindingGroup">The control scheme of the keyboard bindings</param>
    /// <returns>Wether the operation was successful</returns>
    public bool AddBackKeyboardBindings(string bindingGroup)
    {
        if (!removedKeyboardBindings.ContainsKey(bindingGroup)) return false;

        //Delay the adding of actions so that the key press is not processes twice in a frame and joining the player back
        //as soon as they left
        foreach (var binding in removedKeyboardBindings[bindingGroup]) joinAction.AddBinding(binding);

        return true;
    }

    //Check wether the device performing the action is already assigned with a player, if not create a new player,
    private void CreatePlayerIfPossible(InputAction.CallbackContext context)
    {
        InputDevice device = context.control.device;

        //Get a reference to the player registry
        var playerRegistry = ServiceLocator.GetService<PlayerRegistry>();

        //If a player with the given device already exists ignore it
        if (playerRegistry.DoesPlayerWithDeviceExist(device)) return;

        MinigamePlayer player;

        //Check wether the device that triggered the joinAction is the keyboard if so remove the binding from the inputAciton
        //and create a player using the control scheme of the binding that was used to trigger the event
        //If the device was not the keyboard just create a player and figure out the control scheme later
        if (device == Keyboard.current)
        {
            var controlScheme = RemoveBindingFromJoining(context);
            player = playerRegistry.CreatePlayerWithDevice(device, true, controlScheme);
        }
        else
        {
            player = playerRegistry.CreatePlayerWithDevice(device);
        }

        //Trigger the event with the created player
        OnPlayerJoin?.Invoke(player);

        player.transform.position = UnityEngine.Vector3.up;
    }

    private string RemoveBindingFromJoining(InputAction.CallbackContext context)
    {
        //Get the index of the used binding and using the index get the group (control scheme) for the binding
        int bindingIndex = context.action.GetBindingIndexForControl(context.control);
        string bindingGroup = GetGroupForBinding(joinAction.ChangeBinding(bindingIndex).binding);

        //Create a list of all the bindings' effective paths to be removed, this makes it easy to compare with the inputAction bindings
        List<string> bindingsToRemove = new();
        List<InputBinding> removedBindings = new();

        foreach (var binding in inputActions.FindActionMap("Player").bindings)
        {
            if (binding.groups == bindingGroup || binding.groups == ';' + bindingGroup) bindingsToRemove.Add(binding.effectivePath);
        }

        //Check each binding, if the list contains the effective path of the binding erase it, effectively stopping it from being checked
        for (int i = joinAction.bindings.Count - 1; i >= 0; i--)
        {
            if (bindingsToRemove.Contains(joinAction.bindings[i].effectivePath))
            {
                removedBindings.Add(joinAction.bindings[i]);
                joinAction.ChangeBinding(i).Erase();
            }
        }

        //Store the removed keyboard bindings so we can add it back when a keyboard player leaves
        if (!removedKeyboardBindings.ContainsKey(bindingGroup)) removedKeyboardBindings.Add(bindingGroup, removedBindings.ToArray());

        //Return the group (control scheme) of the binding
        return bindingGroup;
    }

    private string GetGroupForBinding(InputBinding binding)
    {
        //Check each binding, if one matches return its group (control scheme)
        foreach (var currentBinding in inputActions.FindActionMap("Player").bindings)
        {
            if (currentBinding.effectivePath == binding.effectivePath)
            {
                return currentBinding.groups[0] == ';' ? currentBinding.groups[1..currentBinding.groups.Length] : currentBinding.groups;
            }
        }

        //If none was found the input asset is set up incorrectly as a binding is not connected to a group, return null
        Debug.LogError("Binding group was not found for binding!");
        return null;
    }
}
