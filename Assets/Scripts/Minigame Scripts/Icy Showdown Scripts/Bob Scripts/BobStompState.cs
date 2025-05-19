using UnityEngine;

public class BobStompState : BobState
{
    public override void Initialize(params object[] parameters)
    {
        return;
    }

    public override void LoadState(params object[] parameters)
    {
        isStateRunning = true;
    }

    public override void TickState()
    {
        IcePlatformManager.Instance.BreakBrittlePlatforms();
        isStateRunning = false;
    }

    public override void UnloadState()
    {
        
    }
}
