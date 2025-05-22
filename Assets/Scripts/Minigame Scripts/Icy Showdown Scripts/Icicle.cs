using UnityEngine;

public class Icicle : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private IcicleShadow shadow;

    private float initialY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        initialY = transform.position.y;
        shadow = GetComponentInChildren<IcicleShadow>();
    }

    private void Update()
    {
        if (!rigidbody.isKinematic) transform.up = rigidbody.linearVelocity.normalized;
        if (shadow != null)
        {
            shadow.UpdateTime(transform.position.y / initialY);

            var pos = transform.position;
            shadow.transform.position = new(pos.x, 0.05f, pos.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("IcePlatform"))
        {
            GetComponent<Collider>().isTrigger = false;
            rigidbody.isKinematic = true;

            transform.parent = other.transform;

            var pos = transform.position;
            pos.y = 0.2f;
            transform.position = pos;

            Destroy(shadow.gameObject);
        }
        else if (other.gameObject.CompareTag("DeathBarrier"))
        {
            Destroy(gameObject);
        }
    }
}
