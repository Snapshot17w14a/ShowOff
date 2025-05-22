using UnityEngine;

public class Icicle : MonoBehaviour
{
    private new Rigidbody rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!rigidbody.isKinematic) transform.up = rigidbody.linearVelocity.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("IcePlatform"))
        {
            GetComponent<Collider>().isTrigger = false;
            rigidbody.isKinematic = true;

            transform.parent = other.transform;

            var pos = transform.position;
            pos.y = -0.8f;
            transform.position = pos;
        }
        else if (other.gameObject.CompareTag("DeathBarrier"))
        {
            Destroy(gameObject);
        }
    }
}
