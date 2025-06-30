using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class IdleTimer : MonoBehaviour
{
    [SerializeField] private MinigameState idleState;
    [SerializeField] private MinigameHandler handler;

    [SerializeField] private float maxIdleTime = 30;
    private float idleTime;
    private bool isTransitioning = false;

    private readonly List<InputDevice> inputDevices = new();

    private void Start()
    {
        Services.Get<PlayerRegistry>().OnPlayerRegistered += ResetDeviceList;
    }

    void Update()
    {
        idleTime += Time.deltaTime;

        if (Input.anyKey || AnyInput()) idleTime = 0;

        if (idleTime >= maxIdleTime && !isTransitioning)
        {
            isTransitioning = true;
            TransitionController.Instance.TransitionOut(1f, () =>
            {
                idleTime = 0;
                isTransitioning = false;
                handler.LoadState(idleState);
                TransitionController.Instance.TransitionIn(1f);
            });
        }
    }

    private void ResetDeviceList(int id)
    {
        inputDevices.Clear();
        inputDevices.AddRange(InputSystem.devices);
    }
    
    private bool AnyInput()
    {
        foreach (var device in inputDevices)
            if (device.IsPressed()) return true;
        return false;
    }

    private void OnDisable()
    {
        Services.Get<PlayerRegistry>().OnPlayerRegistered -= ResetDeviceList;
    }
}
