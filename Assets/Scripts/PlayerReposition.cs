using System.Collections;
using UnityEngine;

public class PlayerReposition : MonoBehaviour
{
    [SerializeField] private Vector3 reposition;
    [SerializeField] private float flightAndStunDuration;

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

            StartCoroutine(ResetPlayerCollider(collider, flightAndStunDuration));
        }
    }

    private IEnumerator ResetPlayerCollider(Collider colliderToReset, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        colliderToReset.enabled = true;
        colliderToReset.GetComponent<MinigamePlayer>().SetFlightState(false);

        yield return null;
    }
}
