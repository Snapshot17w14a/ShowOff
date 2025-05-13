using UnityEngine;

public class CollectionArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Pickupable pickup = other.GetComponent<Pickupable>();

        if (pickup != null && pickup.IsPickedUp)
        {
            Debug.Log("Treasure deposited");
            Destroy(pickup.gameObject);
        }
    }
}
