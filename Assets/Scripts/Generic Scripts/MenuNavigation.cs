using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private EventSystem eventSystem;
    public InputActionAsset inputActions;
    public InputActionAsset joinAction;
    void Start()
    {
        Initialize();
        eventSystem.SetSelectedGameObject(buttons[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        //Create a list to store all binding used to interact with the players
        List<string> bindings = new();

        //Extract all the bindings in the Player action map from the inputActions asset
        foreach (var binding in inputActions.FindActionMap("UI").bindings)
        {
            string currentBindingString = binding.effectivePath;

            //Some of the bindings are just organizers, meaning they are "Vector2D" and not actual binding, we skip these
            if (currentBindingString[0] != '<') continue;

            //If the binding is not skipped add it to the list of bindings
            bindings.Add(currentBindingString);
        }
    }
}
