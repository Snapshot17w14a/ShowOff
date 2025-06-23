using System.Collections.Generic;
using UnityEngine;

public class PlayerReposition : MonoBehaviour
{
    [SerializeField] private float flightDuration;
    [SerializeField] private float stunDuration;

    private Dictionary<Collider, float> colliderDampeningPair = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerScript = other.GetComponent<MinigamePlayer>();
            playerScript.SetFlightState(true);
            playerScript.TreasureInteraction.DropTreasureRandom();
            playerScript.StunPlayer(stunDuration);

            var rb = other.GetComponent<Rigidbody>();

            rb.linearVelocity = Vector3.zero;

            var platforms = IcePlatformManager.Instance.SelectPlatforms(platform => !platform.IsBroken);
            IcePlatform closestPlatform = null;
            float closestDistance = float.MaxValue;

            foreach (var platform in platforms)
            {
                var dist = Vector3.Distance(other.transform.position, platform.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestPlatform = platform;
                }
            }

            rb.linearVelocity = PathCalculator.CalculateRequiredVelocity(other.transform.position, closestPlatform != null ? closestPlatform.transform.position : Vector3.zero, flightDuration);

            var collider = other.GetComponent<Collider>();
            collider.enabled = false;

            colliderDampeningPair.Add(collider, rb.linearDamping);
            rb.linearDamping = 0;

            Scheduler.Instance.DelayExecution(() =>
            {
                collider.enabled = true;
                collider.GetComponent<MinigamePlayer>().SetFlightState(false);
                collider.GetComponent<Rigidbody>().linearDamping = colliderDampeningPair[collider];
                colliderDampeningPair.Remove(collider);
            }, flightDuration - 0.1f);
        }
    }
}
