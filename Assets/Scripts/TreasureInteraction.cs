using UnityEngine;

public class TreasureInteraction : MonoBehaviour
{
    [SerializeField] private float pickupRange = 2f;
    private Pickupable nearbyPickupable;
    private Pickupable collectedPickupable;

    void Start()
    {
        
    }

    void Update()
    {
        DetectNearbyPickupables();
    }

    private void FixedUpdate()
    {
        if(collectedPickupable != null)
        {
            collectedPickupable.transform.position = transform.position + transform.forward * 1f;
        }
    }

    private void DetectNearbyPickupables()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);

        foreach(Collider hit in hits)
        {
            Pickupable pickUp = hit.GetComponent<Pickupable>();

            if(pickUp != null && !pickUp.IsPickedUp)
            {
                Debug.Log("Found Pickup");
                nearbyPickupable = pickUp;
                break;
            }
        }
    }

    private void PickUp(Pickupable pickUp)
    {
        pickUp.Collect(transform);
        collectedPickupable = pickUp;
    }

    private void Drop()
    {
        collectedPickupable.Drop();
        collectedPickupable = null;
    }

    private void OnGrab()
    {
        if (nearbyPickupable != null && collectedPickupable == null)
        {
            PickUp(nearbyPickupable);
        }
    }
}
