using System;
using UnityEngine;
using UnityEngine.VFX;

public class BobStompState : BobState
{
    private float knockbackRange;
    private float knockbackForce;
    private VisualEffect stompEffect;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 3) throw new Exception("Provided parameters array length was not 3");

        knockbackRange = (float)parameters[0];
        knockbackForce = (float)parameters[1];
        stompEffect = (VisualEffect)parameters[2];
    }

    public override void LoadState(params object[] parameters)
    {
        isStateRunning = true;

        Bob.Instance.Animator.SetTrigger("Stomp");

        Scheduler.Instance.DelayExecution(() =>
        {
            KnockBackPlayers();
            stompEffect.Play();
            AudioManager.PlaySound(ESoundType.Bob, "Stomp", false, 1, 0.3f);
            IcePlatformManager.Instance.BreakBrittlePlatforms();
            Camera.main.GetComponent<PlayerCenterFollow>().ShakeCamera(1f);

            isStateRunning = false;
        }, 0.17f);
    }

    public override void TickState()
    {

    }

    public override void UnloadState()
    {
        stompEffect.Stop();
    }

    private void KnockBackPlayers()
    {
        var players = GameObject.FindObjectsByType<MinigamePlayer>(FindObjectsSortMode.None);

        foreach (var p in players)
        {
            var pos = p.transform.position;
            var dist = pos.magnitude;

            if (dist < knockbackRange)
            {
                if(DifficultyManager.IsEasyMode())
                {
                    p.GetComponent<Rigidbody>().AddForce(pos.normalized * (knockbackForce / 2), ForceMode.Impulse);
                } else
                {
                    p.GetComponent<Rigidbody>().AddForce(pos.normalized * knockbackForce, ForceMode.Impulse);
                }
            }
        }
    }
}
