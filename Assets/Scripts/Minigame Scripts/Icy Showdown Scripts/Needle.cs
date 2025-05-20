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
        transform.up = rb.linearVelocity.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.GetComponent<MinigamePlayer>();
            player.StunPlayer(stunDuration);
            player.DropTreasure();
        }
        else if (other.gameObject.CompareTag("IcePlatform"))
        {
            rb.isKinematic = true;
        }
        else Destroy(gameObject);
    }
}
