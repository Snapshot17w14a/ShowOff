using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private EventSystem eventSystem;
    public InputActionAsset inputActions;

    private int indexPosition = 0;

    private Dictionary<string, InputAction> inputForAction = new();

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

        //For each action create an input action and add the bindings for that action
        foreach (var binding in bindings)
        {
            Debug.Log(binding.action);
            if (!inputForAction.ContainsKey(binding.action))
                inputForAction.Add(binding.action, new InputAction(name: binding.action));

            inputForAction[binding.action].AddBinding(binding);
        }

        //Add the method to be called each time any connected device performs an action with any relevant binding
        foreach(var inputAction in inputForAction.Values) inputAction.started += context => NavHandler(context);

        ServiceLocator.GetService<PauseManager>().OnPaused += SetActive;
    }

    private void NavHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != ServiceLocator.GetService<PauseManager>().PauseingUserDevice) return;

        var kvp = GetKvpOfInput(ctx.action);

        if (kvp.Key == "Confirm")
        {
            buttons[indexPosition % 3].GetComponent<Button>().onClick.Invoke();
            return;
        }

        indexPosition += kvp.Key == "NavUp" ? -1 : 1;

        if (indexPosition < 0) indexPosition = 3 + indexPosition;

        eventSystem.SetSelectedGameObject(buttons[indexPosition % buttons.Length]);
    }

    private void SetActive(bool state, int playerId)
    {
        foreach(var inputAction in inputForAction.Values)
        {
            if (state) inputAction.Enable();
            else inputAction.Disable();
        }
    }

    private void OnDisable()
    {
        ServiceLocator.GetService<PauseManager>().OnPaused -= SetActive;
    }

    private KeyValuePair<string, InputAction> GetKvpOfInput(InputAction action) => inputForAction.Where(kvp => kvp.Value == action).First();
}
