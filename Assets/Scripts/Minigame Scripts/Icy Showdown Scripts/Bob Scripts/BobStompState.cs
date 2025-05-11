using UnityEngine;

public class BobStompState : BobState
{
    public override void LoadState(params object[] parameters)
    {
        isStateRunning = true;
    }

    public override void TickState()
    {
        Debug.Log("STOMP!");
        IcePlatformManager.Instance.BreakBrittlePlatforms();
        isStateRunning = false;
    }

    public override void UnloadState()
    {
        
    }
}
