using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TreasureInteraction : MonoBehaviour
{
    public event Action OnTreasureDelivered;

    [SerializeField] private Transform holdPoint;
    [SerializeField] private Pickupable treasurePrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float droppedTreasureDespawnTime = 10f;
    [SerializeField] private float spawnRange = 1f;

    private MinigamePlayer miniGamePlayer;
    private Pickupable collectedPickupable;
    private bool isInTreasureZone = false;
    private bool isInCollectionZone = false;
    private Minecart currentMinecart;

    public bool IsHoldingItem => collectedPickupable != null;

    private void Awake()
    {
        miniGamePlayer = GetComponent<MinigamePlayer>();
    }

    private void Update()
    {
        if (isInCollectionZone && collectedPickupable != null)
        {
            DeliverTreasure();
            isInCollectionZone = false;
        }
    }

    private void CollectTreasure()
    {
        Pickupable treasure = Instantiate(treasurePrefab, holdPoint.position, Quaternion.identity);
        Rigidbody TreasureRB = treasure.GetComponent<Rigidbody>();
        TreasureRB.isKinematic = true;

        if (treasure != null)
        {
            treasure.Collect(holdPoint);
            treasure.GetComponent<Collider>().enabled = false;
            collectedPickupable = treasure;
        }
    }

    private void DeliverTreasure()
    {
        if (collectedPickupable != null)
        {
            Destroy(collectedPickupable.gameObject);
            collectedPickupable = null;
            OnTreasureDelivered?.Invoke();
            Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            Pickupable treasure = Instantiate(treasurePrefab, spawnPoint, Quaternion.identity);
            Rigidbody treasureRB = treasure.GetComponent<Rigidbody>();
            treasureRB.isKinematic = false;
            treasureRB.linearVelocity = PathCalculator.CalculateRequiredVelocity(spawnPoint, currentMinecart.transform.position, 1f);
        }
    }

    //Drops the treasure randomly around the player or whenever goes outside the ice, drops it back on the ice.
    public void DropTreasureRandom()
    {
        if (collectedPickupable != null)
        {
            Destroy(collectedPickupable.gameObject);
            collectedPickupable = null;

            Vector3 position = transform.position;
            Vector3 randomDirection = new Vector3(Random.Range(-spawnRange, spawnRange), 0f, Random.Range(-spawnRange, spawnRange));
            Vector3 spawnPosition = position + randomDirection;

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                //for some reason if I increase the timeToTarget above 0.6 (whenever I get stunned by bob the gem sinks into the ground).
                SpawnAnimation(hit, 0.5f, 0.6f);
            }
        }
    }

    //Used to manually drop the treasure infront of the player
    private void DropTreasure()
    {
        if (collectedPickupable != null)
        {
            Vector3 position = transform.position;
            Vector3 dropPosition = transform.forward / 1.5f;
            Vector3 spawnPosition = position + dropPosition;

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                SpawnAnimation(hit, 0.3f, 0.3f);
            }
        }
    }

    private void SpawnAnimation(NavMeshHit hit, float playerYOffset, float timeToTarget)
    {
        Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + playerYOffset, transform.position.z);
        Pickupable treasure = Instantiate(treasurePrefab, spawnPoint, Quaternion.identity);
        Rigidbody treasureRB = treasure.GetComponent<Rigidbody>();
        treasureRB.isKinematic = false;
        treasureRB.linearVelocity = PathCalculator.CalculateRequiredVelocity(spawnPoint, hit.position, timeToTarget);
        treasure.DespawnAfter(droppedTreasureDespawnTime);
    }

    //Destroys the treasure whenever of the player holding the treasure whenever stunned and gives it to the other player - works with CollectTreasureDirect
    public void DropTreasureInstant()
    {
        if (collectedPickupable != null)
        {
            Destroy(collectedPickupable.gameObject);
            collectedPickupable = null;
        }
    }

    //See above
    public void CollectTreasureDirect()
    {
        if (collectedPickupable != null)
        {
            return;
        }

        Pickupable treasure = Instantiate(treasurePrefab, holdPoint.position, Quaternion.identity);

        if (treasure != null)
        {
            treasure.Collect(holdPoint);
            treasure.GetComponent<Collider>().enabled = false;
            collectedPickupable = treasure;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TreasureZone>() != null)
        {
            isInTreasureZone = true;
            EnableButtonIndicator(true);
        }

        if (other.GetComponent<CollectionZone>() != null)
        {
            Minecart minecart = other.GetComponent<Minecart>();
            if (minecart != null)
            {
                currentMinecart = minecart;
            }
            isInCollectionZone = true;
        }

        if (other.GetComponent<Pickupable>() != null)
        {
            if (!IsPlayerStunned() && collectedPickupable == null)
            {
                CollectTreasure();
                Destroy(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<TreasureZone>() != null)
        {
            isInTreasureZone = false;
            EnableButtonIndicator(false);
        }

        if (other.GetComponent<CollectionZone>() != null)
        {
            Minecart minecart = other.GetComponent<Minecart>();
            if (minecart != null && minecart == currentMinecart)
            {
                currentMinecart = null;
            }
            isInCollectionZone = false;
        }
    }

    private bool IsPlayerStunned()
    {
        return miniGamePlayer != null && miniGamePlayer.IsStunned;
    }

    private void EnableButtonIndicator(bool state)
    {
        spriteRenderer.gameObject.SetActive(state);
    }

    private void OnGrab()
    {
        if (isInTreasureZone && collectedPickupable == null)
        {
            CollectTreasure();
        }
        else if (collectedPickupable != null)
        {
            DropTreasure();
            Destroy(collectedPickupable.gameObject);
            collectedPickupable = null;
        }
    }
}
