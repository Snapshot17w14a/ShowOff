using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAutoJoin : Service
{
    public InputActionAsset inputActions;

    public bool AutoJoinPlayers { get; set; } = true;

    private InputAction joinAction;

    public override void InitializeService()
    {
        List<string> bindings = new();
        //string fullBindingString = "";

        foreach (var binding in inputActions.FindActionMap("Player").bindings)
        {
            string currentBindingString = binding.effectivePath;
            if (currentBindingString[0] != '<') continue;
            //fullBindingString += (currentBindingString + ", ");
            bindings.Add(currentBindingString);
        }

        //fullBindingString = fullBindingString[..^2];

        //Debug.Log(fullBindingString);

        joinAction = new(
            name: "JoinAction",
            binding: "",
            interactions: "",
            processors: ""
        );

        foreach (var binding in bindings) joinAction.AddBinding(binding);

        joinAction.performed += context => /*Debug.Log(context.control.device.name);*/ CreatePlayerIfPossible(context.control.device);

        joinAction.Enable();
    }

    private void CreatePlayerIfPossible(InputDevice device)
    {
        var playerRegistry = ServiceLocator.GetService<PlayerRegistry>();
        if (playerRegistry.DoesPlayerWithDeviceExist(device)) return;
        playerRegistry.CreatePlayerWithDevice(device);
    }
}
