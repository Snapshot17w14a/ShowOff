using System;
using System.Collections;
using UnityEngine;

public enum PickupType
{
    Small,
    Large
}

public class Pickupable : MonoBehaviour
{
    public event Action<Pickupable> OnPickupableDespawnedEvent;
    public event Action<Pickupable, int> OnPickupableEnteredMinecartEvent;
    public event Action<Pickupable> OnGroundTouched;

    public PickupType PickupType => pickupType;
    public int Worth => worth;
    public bool IsPickedUp { get; private set; } = false;

    [SerializeField] private Material[] gemMaterials;
    [SerializeField] private PickupType pickupType;
    [SerializeField] private int gemPickUpSize = 12;
    [SerializeField] private int worth = 1;

    private Rigidbody rb;
    private new Collider collider;
    private MeshRenderer meshRenderer;
    private Coroutine despawnCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void DespawnAfter(float time)
    {
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
        }

        despawnCoroutine = StartCoroutine(DespawnCoroutine(time));
    }

    private IEnumerator DespawnCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        OnPickupableDespawnedEvent?.Invoke(this);
        Destroy(gameObject);
    }

    public void CancelDespawn()
    {
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            despawnCoroutine = null;
        }
    }

    public void Collect(Transform parent)
    {
        CancelDespawn();
        IsPickedUp = true;
        SetKinematic(true);
        gameObject.transform.SetParent(parent, false);
        //This is fucking ugly, but I was over it making it work with math
        transform.localScale = new Vector3(gemPickUpSize, gemPickUpSize, gemPickUpSize);
        gameObject.transform.localPosition = Vector3.zero;
    }

    private void Despawn()
    {

        Destroy(gameObject);
        OnPickupableDespawnedEvent?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IcePlatform>() != null)
        {
            SetKinematic(true);
            OnGroundTouched?.Invoke(this);
        }

        else if (other.GetComponent<Minecart>() != null)
        {

            Minecart minecart = other.GetComponent<Minecart>();
            minecart.AddGem();
            OnPickupableEnteredMinecartEvent?.Invoke(this, worth);
            Despawn();

        }

        else if (other.CompareTag("IcePlatform"))
        {
            SetKinematic(true);
            OnGroundTouched?.Invoke(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        SetKinematic(true);
        collider.isTrigger = true;
    }

    public void CalculateVelocity(Vector3 spawnPoint, Vector3 position, float speed)
    {
        rb.linearVelocity = PathCalculator.CalculateRequiredVelocity(spawnPoint, position, speed);
    }

    public void SetKinematic(bool state)
    {
        rb.isKinematic = state;
    }

    public void SetTrigger(bool state)
    {
        collider.isTrigger = state;
    }

    public void SetCollider(bool state)
    {
        collider.enabled = state;
    }

    public void SetRandomMaterial()
    {
        if (gemMaterials != null && gemMaterials.Length < 1)
        {
            return;
        }

        Material randomMaterial = gemMaterials[UnityEngine.Random.Range(0, gemMaterials.Length)];
        meshRenderer.material = randomMaterial;
    }
}
