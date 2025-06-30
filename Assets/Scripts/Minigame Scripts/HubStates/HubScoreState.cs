using System.Collections.Generic;
using UnityEngine;

public class HubScoreState : MinigameState
{
    [SerializeField] private List<Material> ditherMaterials;
    [SerializeField] private GameObject cloudPlatform;

    [SerializeField] private CameraLerp defaultCameraLerp;

    public override void LoadState()
    {
        base.LoadState();
        foreach (var m in ditherMaterials) m.SetFloat("_Strength", 1);
    }

    public override void UnloadState()
    {
        base.UnloadState();
        ((HubDefaultState)nextMinigameState).isAfterPodiums = true;
        defaultCameraLerp.StartLerping();
    }

    public void SkipPodiumStage()
    {
        Scheduler.Instance.Lerp(t =>
        {
            foreach (var m in ditherMaterials) m.SetFloat("_Strength", 1 - t);
        }, 2f, () => {
            MinigameHandler.Instance.LoadState(nextMinigameState);
            Services.Get<PlayerRegistry>().ExecuteForEachPlayer(player =>
            {
                player.GetPlayerAnimator.SetTrigger("Stun");
                player.GetComponent<Rigidbody>().linearDamping = 0f;
            });
            defaultCameraLerp.callback = () => Services.Get<PlayerRegistry>().ExecuteForEachPlayer(player =>
            {
                player.GetPlayerAnimator.SetTrigger("StunOver");
                player.GetComponent<Rigidbody>().linearDamping = 2f;
            });
            Scheduler.Instance.DelayExecution(() =>
            {
                foreach (var m in ditherMaterials) m.SetFloat("_Strength", 1);
            }, 4f);
        });
        
    }

    public void AddMaterial(Material ditherMaterial)
    {
        ditherMaterials.Add(ditherMaterial);
    }
}
