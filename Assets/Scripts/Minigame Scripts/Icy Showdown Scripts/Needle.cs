using UnityEngine;

public class Needle : MonoBehaviour
{
    [SerializeField] private float stunDuration = 1f;
    [SerializeField] private float timeoutSeconds = 5f;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, timeoutSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        if (!rb.isKinematic) transform.up = rb.linearVelocity.normalized;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.GetComponent<MinigamePlayer>();
            player.StunPlayer(stunDuration);
            player.TreasureInteraction.DropTreasureRandom();
        }
        else if (other.gameObject.CompareTag("IcePlatform"))
        {
            rb.isKinematic = true;
            GetComponent<Collider>().enabled = false;
        }
    }
}
