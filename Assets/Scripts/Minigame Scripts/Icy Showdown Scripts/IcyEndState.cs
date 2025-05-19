public class IcyEndState : MinigameState
{
    public override void TickState()
    {
        base.TickState();
    }

    public override void LoadState()
    {
        base.LoadState();
        //TransitionController.Instance.TransitionOut("HubScene");
        //MinigameHandler.Instance.ChangeStateInSeconds(1, nextMinigameState);
    }
}
