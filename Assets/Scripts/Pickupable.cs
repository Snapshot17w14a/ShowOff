using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public bool IsPickedUp {  get; private set; } = false;
   
    private float despawnTime;
    private bool isDespawning;

    public void DespawnAfter(float timeUntilDespawn)
    {
        despawnTime = Time.time + timeUntilDespawn;
        isDespawning = true;
    }

    public void Collect(Transform parent)
    {
        IsPickedUp = true;
        gameObject.transform.SetParent(parent, false);
        gameObject.transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        IsPickedUp = false;
        transform.SetParent(null);
    }

    private void Update()
    {
        if (isDespawning && Time.time >= despawnTime)
        {
            Destroy(gameObject);
            isDespawning = false;
        }
    }
}
