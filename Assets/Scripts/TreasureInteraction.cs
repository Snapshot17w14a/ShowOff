using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TreasureInteraction : MonoBehaviour
{
    public event Action OnTreasureDelivered;

    [SerializeField] private Transform holdPoint;
    [SerializeField] private Pickupable treasurePrefab;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private float dropRange = 0.2f;
    [SerializeField] private float droppedTreasureDespawnTime = 10f;
    [SerializeField] private float spawnRange = 0.5f;
    [SerializeField] private float treasurePickUpCooldown = 2f;

    private MinigamePlayer miniGamePlayer;
    private Pickupable collectedPickupable;
    private bool isInTreasureZone = false;
    private bool isInCollectionZone = false;

    private void Awake()
    {
        miniGamePlayer = GetComponent<MinigamePlayer>();
    }

    private void Update()
    {
        if (isInCollectionZone && collectedPickupable != null)
        {
            DeliverTreasure();
        }
    }

    private void CollectTreasure()
    {
        Pickupable treasure = Instantiate(treasurePrefab, holdPoint.position, Quaternion.identity);

        if (treasure != null)
        {
            treasure.Collect(holdPoint);
            collectedPickupable = treasure;
            Debug.Log("Collected treasure!");
        }
    }

    private void DeliverTreasure()
    {
        if (collectedPickupable != null)
        {
            Destroy(collectedPickupable.gameObject);
            collectedPickupable = null;
            Debug.Log("Treasure Delivered!");
            OnTreasureDelivered?.Invoke();
        }
    }

    public void DropTreasure()
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
                Pickupable treasure = Instantiate(treasurePrefab, hit.position, Quaternion.identity);
                treasure.DespawnAfter(droppedTreasureDespawnTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TreasureZone>() != null)
        {
            isInTreasureZone = true;
        }

        if (other.GetComponent<CollectionZone>() != null)
        {
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
        }

        if (other.GetComponent<CollectionZone>() != null)
        {
            isInCollectionZone = false;
        }
    }

    private bool IsPlayerStunned()
    {
        return miniGamePlayer != null && miniGamePlayer.IsStunned;
    }

    private void OnGrab()
    {
        if (isInTreasureZone && collectedPickupable == null)
        {
            CollectTreasure();
        }
    }
}
