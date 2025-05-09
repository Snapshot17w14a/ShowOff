using UnityEngine;

public abstract class BobState
{
    public abstract void LoadState(params object[] parameters);

    public abstract void UnloadState();

    public abstract void TickState();

    public bool IsStateRunning => isStateRunning;
    protected bool isStateRunning = false;
}
