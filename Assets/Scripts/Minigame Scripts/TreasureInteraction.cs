using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TreasureInteraction : MonoBehaviour
{
    public event Action<int> OnTreasureDelivered;

    [Header("Prefabs")]
    [SerializeField] private Pickupable treasurePrefab;
    [SerializeField] private Pickupable largeTreasurePrefab;

    [Header("Large Gem Settings")]
    [SerializeField, Range(1, 100)] private int spawnChance;
    [SerializeField, Range(100, 200)] private int movementSpeedPenalty;

    [Header("Other stuff")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float droppedTreasureDespawnTime = 10f;
    [SerializeField] private float pickUpCooldown = 2f;

    public Pickupable CollectedPickupable => collectedPickupable;
    public int MovementSpeedPenalty => movementSpeedPenalty;

    private float spawnRange = 1f;
    private float _pickUpCooldown;
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

    private void Start()
    {
        _pickUpCooldown = pickUpCooldown;
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
        if (!PauseManager.isPaused)
        {

            int roll = UnityEngine.Random.Range(1, 101);

            if (roll <= spawnChance)
            {
                Pickupable largeTreasure = Instantiate(largeTreasurePrefab, holdPoint.position, Quaternion.identity);
                largeTreasure.OnPickupableDespawnedEvent += HandleTreasureDespawned;
                largeTreasure.SetKinematic(true);

                if (largeTreasure != null)
                {
                    largeTreasure.Collect(holdPoint);
                    largeTreasure.GetComponent<Collider>().enabled = false;
                    collectedPickupable = largeTreasure;
                }
            }
            else
            {
                Pickupable treasure = Instantiate(treasurePrefab, holdPoint.position, Quaternion.identity);
                treasure.OnPickupableDespawnedEvent += HandleTreasureDespawned;
                treasure.SetKinematic(true);

                if (treasure != null)
                {
                    treasure.Collect(holdPoint);
                    treasure.GetComponent<Collider>().enabled = false;
                    collectedPickupable = treasure;
                }
            }
        }
    }

    private void CollectTreasureFromGround(Pickupable pickupable)
    {
        if (!PauseManager.isPaused)
        {
            Pickupable droppedTreasure = Instantiate(pickupable, holdPoint.position, Quaternion.identity);
            droppedTreasure.OnPickupableDespawnedEvent += HandleTreasureDespawned;
            droppedTreasure.SetKinematic(true);

            if (pickupable != null)
            {
                droppedTreasure.Collect(holdPoint);
                droppedTreasure.GetComponent<Collider>().enabled = false;
                collectedPickupable = droppedTreasure;
            }
        }
    }

    private void DeliverTreasure()
    {
        if (collectedPickupable != null)
        {
            Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            Pickupable spawnPrefab = GetTreasurePrefab(collectedPickupable.PickupType);
            Pickupable treasure = Instantiate(spawnPrefab, spawnPoint, Quaternion.identity);
            treasure.SetKinematic(false);
            treasure.CalculateVelocity(spawnPoint, currentMinecart.transform.position, 1f);

            OnTreasureDelivered?.Invoke(collectedPickupable.Worth);
            Destroy(collectedPickupable.gameObject);
            collectedPickupable = null;
        }
    }

    //Drops the treasure randomly around the player or whenever goes outside the ice, drops it back on the ice.
    public void DropTreasureRandom()
    {
        if (collectedPickupable != null)
        {
            Vector3 position = transform.position;
            Vector3 randomDirection = new Vector3(Random.Range(-spawnRange, spawnRange), 0f, Random.Range(-spawnRange, spawnRange));
            Vector3 spawnPosition = position + randomDirection;

            if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                //for some reason if I increase the timeToTarget above 0.6 (whenever I get stunned by bob the gem sinks into the ground).
                SpawnAnimation(hit, 0.5f, 1f);
            }
            Destroy(collectedPickupable.gameObject);
            collectedPickupable = null;
        }
    }

    private void SpawnAnimation(NavMeshHit hit, float playerYOffset, float timeToTarget)
    {
        Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + playerYOffset, transform.position.z);
        Pickupable spawnPrefab = GetTreasurePrefab(collectedPickupable.PickupType);
        Pickupable treasure = Instantiate(spawnPrefab, spawnPoint, Quaternion.identity);
        treasure.SetKinematic(false);
        treasure.SetTrigger(false);
        treasure.CalculateVelocity(spawnPoint, hit.position, timeToTarget);
        treasure.DespawnAfter(droppedTreasureDespawnTime);
    }

    //Used to manually drop the treasure infront of the player
    private void DropTreasure()
    {
        if (collectedPickupable != null)
        {
            Vector3 position = transform.position;
            Vector3 dropPosition = transform.forward / 1.5f;
            Vector3 spawnPosition = position + dropPosition;
            Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            Rigidbody rb = GetComponent<Rigidbody>();
            Pickupable spawnPrefab = GetTreasurePrefab(collectedPickupable.PickupType);
            Pickupable treasure = Instantiate(spawnPrefab, spawnPoint, Quaternion.identity);
            treasure.OnPickupableEnteredMinecartEvent += HandleTreasureEnteredMinecart;
            treasure.SetKinematic(false);
            treasure.CalculateVelocity(spawnPoint, spawnPosition + (rb.linearVelocity / 3f), 0.3f);
            treasure.DespawnAfter(droppedTreasureDespawnTime);
        }
    }

    public Pickupable GetTreasurePrefab(PickupType type)
    {
        switch (collectedPickupable.PickupType)
        {
            case PickupType.Small:
                return treasurePrefab;
            case PickupType.Large:
                return largeTreasurePrefab;
            default: throw new NotImplementedException(collectedPickupable.PickupType.ToString());
        }
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
        Pickupable spawnPrefab = GetTreasurePrefab(collectedPickupable.PickupType);
        Pickupable treasure = Instantiate(spawnPrefab, holdPoint.position, Quaternion.identity);

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
                Pickupable pickupable = other.GetComponent<Pickupable>();
                CollectTreasureFromGround(pickupable);
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
        if (isInTreasureZone && collectedPickupable == null && Time.time > pickUpCooldown)
        {
            CollectTreasure();
            pickUpCooldown = Time.time + _pickUpCooldown;
        }
        else if (collectedPickupable != null)
        {
            DropTreasure();
            Destroy(collectedPickupable.gameObject);
            collectedPickupable = null;
        }
    }

    private void HandleTreasureDespawned(Pickupable pickupable)
    {
        pickupable.OnPickupableEnteredMinecartEvent -= HandleTreasureEnteredMinecart;
        pickupable.OnPickupableDespawnedEvent -= HandleTreasureDespawned;
    }

    private void HandleTreasureEnteredMinecart(Pickupable pickupable, int value)
    {
        OnTreasureDelivered?.Invoke(value);
    }
}
