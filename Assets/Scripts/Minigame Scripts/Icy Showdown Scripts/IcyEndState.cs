using UnityEngine;

public class IcyEndState : MinigameState
{
    public override void TickState()
    {
        base.TickState();
        Debug.Log("EndState");
    }
}
