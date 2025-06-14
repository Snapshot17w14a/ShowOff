using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private EventSystem eventSystem;
    public InputActionAsset inputActions;
    public InputAction navAction;

    void Start()
    {
        Initialize();
        eventSystem.SetSelectedGameObject(buttons[0]);
    }

    private void Initialize()
    {
        //Create a list to store all binding used to interact with the players
        List<InputBinding> bindings = new();

        if(inputActions != null)
        {
            //Extract all the bindings in the Player action map from the inputActions asset
            foreach (var binding in inputActions.FindActionMap("UI").bindings)
            {
                string currentBindingString = binding.effectivePath;

                //Some of the bindings are just organizers, meaning they are "Vector2D" and not actual binding, we skip these
                if (currentBindingString[0] != '<') continue;

                //If the binding is not skipped add it to the list of bindings
                bindings.Add(binding);
            }
        }

        //Create an InputAction that will hold the bindings to listen for
        navAction = new(
            name: "NavAction",
            binding: "",
            interactions: "",
            processors: ""
        );

        //Add each binding to the joinAction
        foreach (var binding in bindings) navAction.AddBinding(binding.effectivePath, groups: binding.groups);

        //Add the method to be called each time any connected device performs an action with any relevant binding
        //navAction.performed += context => NavHandler(context);

        //navAction.Enable();
    }

    private void NavHandler(InputAction.CallbackContext ctx)
    {
    }
}
