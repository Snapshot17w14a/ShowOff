using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class TreasureInteraction : MonoBehaviour
{
    public event Action<int> OnTreasureDelivered;

    [Header("Prefabs")]
    [SerializeField] private Pickupable[] treasurePrefabs;
    [SerializeField] private Pickupable[] largeTreasurePrefabs;

    [Header("Large Gem Settings")]
    [SerializeField, Range(1, 100)] private int spawnChance;
    [SerializeField, Range(0, 100)] private int movementSpeedPenalty;
    [SerializeField, Range(1, 5)] private float throwPenalty = 2.5f;

    [Header("Other stuff")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float droppedTreasureDespawnTime = 10f;
    [SerializeField] private float pickUpCooldown = 2f;

    [SerializeField] private Animator animator;

    public Pickupable CollectedPickupable => collectedPickupable;
    public int MovementSpeedPenalty => movementSpeedPenalty;

    private float spawnRange = 1f;
    private float _pickUpCooldown;
    private MinigamePlayer miniGamePlayer;
    private Pickupable collectedPickupable;
    private Pickupable nearbyPickable;
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
                Pickupable largeTreasure = Instantiate(largeTreasurePrefabs[UnityEngine.Random.Range(0, largeTreasurePrefabs.Length)], holdPoint.position, Quaternion.identity);
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
                Pickupable treasure = Instantiate(treasurePrefabs[UnityEngine.Random.Range(0, treasurePrefabs.Length)], holdPoint.position, Quaternion.identity);
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
            SetGem(pickupable, holdPoint.transform.position);
            pickupable.OnPickupableDespawnedEvent += HandleTreasureDespawned;
            pickupable.SetKinematic(true);

            if (pickupable != null)
            {
                pickupable.Collect(holdPoint);
                pickupable.GetComponent<Collider>().enabled = false;
                collectedPickupable = pickupable;
            }
        }
    }

    private void SetGem(Pickupable gem, Vector3 position)
    {
        gem.transform.position = position;
        gem.transform.rotation = Quaternion.identity;
    }

    private void DeliverTreasure()
    {
        if (collectedPickupable != null)
        {
            Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            SetGem(collectedPickupable, spawnPoint);
            collectedPickupable.SetKinematic(false);
            collectedPickupable.SetCollider(true);
            collectedPickupable.transform.SetParent(null, true);
            collectedPickupable.CalculateVelocity(spawnPoint, currentMinecart.transform.position, 1f);
            OnTreasureDelivered?.Invoke(collectedPickupable.Worth);
            collectedPickupable = null;
            animator.SetBool("IsHolding", false);
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
                SpawnAnimation(hit, 0.5f, 1f, collectedPickupable);
            }
            collectedPickupable = null;
        }
    }

    private void SpawnAnimation(NavMeshHit hit, float playerYOffset, float timeToTarget, Pickupable gem)
    {
        Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + playerYOffset, transform.position.z);
        SetGem(gem, spawnPoint);
        gem.SetCollider(true);
        gem.SetKinematic(false);
        gem.SetTrigger(true);
        gem.transform.SetParent(null, true);
        gem.CalculateVelocity(spawnPoint, hit.position, timeToTarget);
        gem.DespawnAfter(droppedTreasureDespawnTime);
    }

    //Used to manually drop the treasure infront of the player
    private void DropTreasure()
    {
        if (collectedPickupable != null)
        {
            Vector3 position = transform.position;
            Vector3 dropPosition = transform.forward / (collectedPickupable.Worth > 1f ? throwPenalty : 1.5f);

            Vector3 spawnPosition = position + dropPosition;
            Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            Rigidbody rb = GetComponent<Rigidbody>();
            SetGem(collectedPickupable, spawnPoint);
            collectedPickupable.SetCollider(true);
            collectedPickupable.transform.SetParent(null, true);
            collectedPickupable.SetKinematic(false);
            collectedPickupable.OnGroundTouched += HandleOnGroundTouched;
            collectedPickupable.OnPickupableEnteredMinecartEvent += HandleTreasureEnteredMinecart;
            collectedPickupable.CalculateVelocity(spawnPoint, spawnPosition + (rb.linearVelocity / 3f), 0.3f);
            collectedPickupable.DespawnAfter(droppedTreasureDespawnTime);
            collectedPickupable = null;
        }
    }

    public Pickupable GetTreasurePrefab(PickupType type)
    {
        switch (type)
        {
            case PickupType.Small:
                return treasurePrefabs[UnityEngine.Random.Range(0, treasurePrefabs.Length)];
            case PickupType.Large:
                return largeTreasurePrefabs[UnityEngine.Random.Range(0, largeTreasurePrefabs.Length)];
            default: throw new NotImplementedException(collectedPickupable.PickupType.ToString());
        }
    }



    //Destroys the treasure whenever of the player holding the treasure whenever stunned and gives it to the other player - works with CollectTreasureDirect
    public void DropTreasureInstant()
    {
        if (collectedPickupable != null)
        {
            collectedPickupable = null;
        }
    }

    //See above
    public void CollectTreasureDirect(PickupType type, Pickupable gem)
    {
        SetGem(gem, holdPoint.transform.position);
        gem.SetKinematic(true);
        gem.transform.SetParent(this.transform, true);

        if (gem != null)
        {
            gem.Collect(holdPoint);
            gem.GetComponent<Collider>().enabled = false;
            collectedPickupable = gem;
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
            EnableButtonIndicator(true);

            if (!IsPlayerStunned() && collectedPickupable == null)
            {
                Pickupable pickupable = other.GetComponent<Pickupable>();
                nearbyPickable = pickupable;
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

        if(other.GetComponent<Pickupable>() != null)
        {
            nearbyPickable = null;
            EnableButtonIndicator(false);
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

    public void OnGrab(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (isInTreasureZone && collectedPickupable == null && Time.time > pickUpCooldown)
        {
            CollectTreasure();
            animator.SetTrigger("PickUp");
            animator.SetBool("IsHolding", true);
            pickUpCooldown = Time.time + _pickUpCooldown;
        } else if(nearbyPickable != null && collectedPickupable == null)
        {
            CollectTreasureFromGround(nearbyPickable);
            animator.SetTrigger("PickUp");
            animator.SetBool("IsHolding", true);
            nearbyPickable = null;
            EnableButtonIndicator(false);
        }
        else if (collectedPickupable != null)
        {
            DropTreasure();
            animator.SetBool("IsHolding", false);
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

    private void HandleOnGroundTouched(Pickupable gem)
    {
        gem.OnPickupableEnteredMinecartEvent -= HandleTreasureEnteredMinecart;
    }
}
