using UnityEngine;

public class Icicle : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private IcicleShadow shadow;

    [SerializeField] private Material[] crystalMaterials;

    private float initialY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        initialY = transform.position.y;
        shadow = GetComponentInChildren<IcicleShadow>();
        GetComponentInChildren<MeshRenderer>().material = crystalMaterials[Random.Range(0, crystalMaterials.Length)];
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
            AudioManager.PlaySound(ESoundType.Bob, "Crystal_Crash", true, 1, 0.7f);
            if (Camera.main.TryGetComponent<PlayerCenterFollow>(out var pcf)) pcf.ShakeCamera(0.39f);

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
