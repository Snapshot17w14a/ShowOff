using System.Collections;
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
            playerScript.DropTreasure();
            playerScript.StunPlayer(stunDuration);

            var rb = other.GetComponent<Rigidbody>();

            rb.linearVelocity = Vector3.zero;

            var platforms = IcePlatformManager.Instance.NonBrokenPlatforms;
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

            StartCoroutine(ResetPlayerCollider(collider, flightDuration - 0.1f));
        }
    }

    private IEnumerator ResetPlayerCollider(Collider colliderToReset, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        colliderToReset.enabled = true;
        colliderToReset.GetComponent<MinigamePlayer>().SetFlightState(false);
        colliderToReset.GetComponent<Rigidbody>().linearDamping = colliderDampeningPair[colliderToReset];
        colliderDampeningPair.Remove(colliderToReset);

        yield return null;
    }
}
