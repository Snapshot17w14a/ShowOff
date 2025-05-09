using UnityEngine;

public class BobIdleState : BobState
{
    public override void LoadState(params object[] parameters)
    {
        Debug.Log(GetType() + "LoadState");
        isStateRunning = true;
    }

    public override void TickState()
    {
        Debug.Log(GetType() + "TickState");
        isStateRunning = false;
    }

    public override void UnloadState()
    {
        Debug.Log(GetType() + "UnloadState");
    }
}
