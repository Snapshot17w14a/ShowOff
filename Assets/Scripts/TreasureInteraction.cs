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
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private float dropRange = 0.2f;
    [SerializeField] private float droppedTreasureDespawnTime = 10f;
    [SerializeField] private float spawnRange = 0.5f;
    [SerializeField] private float treasurePickUpCooldown = 2f;

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
        
        if (treasure != null)
        {
            treasure.Collect(holdPoint);
            treasure.GetComponent<Collider>().enabled = false;
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
            currentMinecart.AddGem();
            OnTreasureDelivered?.Invoke();
        }
    }

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
                Pickupable treasure = Instantiate(treasurePrefab, hit.position, Quaternion.identity);
                treasure.DespawnAfter(droppedTreasureDespawnTime);
            }
        }
    }

    private void DropTreasure()
    {
        if (collectedPickupable != null)
        {
            Vector3 position = transform.position;
            Vector3 dropPosition = transform.forward / 2f;
            Vector3 spawnPosition = position + dropPosition;

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                Pickupable treasure = Instantiate(treasurePrefab, hit.position, Quaternion.identity);
                treasure.DespawnAfter(droppedTreasureDespawnTime);
            }
        }
    }

    public void DropTreasureInstant()
    {
        if(collectedPickupable != null)
        {
            Destroy(collectedPickupable.gameObject);
            collectedPickupable = null;
        }
    }

    public void CollectTreasureDirect()
    {
        if(collectedPickupable != null)
        {
            return;
        }

        Pickupable treasure = Instantiate(treasurePrefab, holdPoint.position, Quaternion.identity);

        if(treasure != null)
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
