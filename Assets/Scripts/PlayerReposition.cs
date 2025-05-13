using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReposition : MonoBehaviour
{
    [SerializeField] private Vector3 reposition;
    [SerializeField] private float flightAndStunDuration;

    private Dictionary<Collider, float> colliderDampeningPair = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerScript = other.GetComponent<MinigamePlayer>();
            playerScript.SetFlightState(true);
            playerScript.StunPlayer(flightAndStunDuration);

            var rb = other.GetComponent<Rigidbody>();

            rb.linearVelocity = Vector3.zero;
            rb.linearVelocity = PathCalculator.CalculateRequiredVelocity(other.transform.position, reposition, flightAndStunDuration);

            var collider = other.GetComponent<Collider>();
            collider.enabled = false;

            colliderDampeningPair.Add(collider, rb.linearDamping);
            rb.linearDamping = 0;

            StartCoroutine(ResetPlayerCollider(collider, flightAndStunDuration));
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
