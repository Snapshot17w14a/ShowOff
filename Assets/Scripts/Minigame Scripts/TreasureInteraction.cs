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
    [SerializeField] private SpriteRenderer inputIndicator;
    [SerializeField] private SpriteRenderer inputIndicatorCooldown;
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
        EventBus<PickupCollected>.OnEvent += RemoveNearbyGem;
    }

    private void CollectTreasure()
    {
        if (!PauseManager.IsPaused)
        {
            int roll = UnityEngine.Random.Range(1, 101);

            if (roll <= spawnChance)
            {
                SpawnTreasure(largeTreasurePrefabs);
                AudioManager.PlaySound(ESoundType.Penguin, "Grab_Gem_Big", false);
            }
            else
            {
                SpawnTreasure(treasurePrefabs);
                AudioManager.PlaySound(ESoundType.Penguin, "Grab_Gem", false);

            }
        }
    }

    private void SpawnTreasure(Pickupable[] pickables)
    {
        Pickupable treasure = Instantiate(pickables[UnityEngine.Random.Range(0, pickables.Length)], holdPoint.position, Quaternion.identity);
        treasure.OnPickupableDespawnedEvent += HandleTreasureDespawned;
        treasure.SetKinematic(true);

        if (treasure != null)
        {
            treasure.Collect(holdPoint);
            treasure.GetComponent<Collider>().enabled = false;
            collectedPickupable = treasure;
        }
    }

    private void CollectTreasureFromGround(Pickupable pickupable)
    {
        if (!PauseManager.IsPaused)
        {
            SetGem(pickupable, holdPoint.transform.position);
            pickupable.OnPickupableDespawnedEvent += HandleTreasureDespawned;
            pickupable.SetKinematic(true);
            pickupable.SetCollider(ColliderState.Flying, false);
            pickupable.SetCollider(ColliderState.Grounded, false);

            if (pickupable != null)
            {
                pickupable.Collect(holdPoint);
                pickupable.GetComponent<Collider>().enabled = false;
                collectedPickupable = pickupable;
                if (collectedPickupable.Worth > 1)
                {
                    AudioManager.PlaySound(ESoundType.Penguin, "Grab_Gem_Big", false);
                }
                else
                {
                    AudioManager.PlaySound(ESoundType.Penguin, "Grab_Gem", false);

                }
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
            collectedPickupable.SetCollider(ColliderState.Flying, false);
            collectedPickupable.transform.SetParent(null, true);
            collectedPickupable.CalculateVelocity(spawnPoint, currentMinecart.transform.position, 1f);
            collectedPickupable.targetMinecart = currentMinecart;
            OnTreasureDelivered?.Invoke(collectedPickupable.Worth);

            var deliveredPickup = collectedPickupable;
            collectedPickupable = null;
            animator.SetBool("IsHolding", false);

            Scheduler.Instance.DelayExecution(() =>
            {
                deliveredPickup.targetMinecart.AddGem();
                Destroy(deliveredPickup.gameObject);
            }, 1f);
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

            collectedPickupable.parentInteration = null;

            collectedPickupable = null;
        }
    }

    private void SpawnAnimation(NavMeshHit hit, float playerYOffset, float timeToTarget, Pickupable gem)
    {
        Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + playerYOffset, transform.position.z);
        SetGem(gem, spawnPoint);
        gem.SetCollider(ColliderState.Flying, true);
        gem.SetKinematic(false);
        gem.SetTrigger(ColliderState.Flying, true);
        gem.transform.SetParent(null, true);
        gem.CalculateVelocity(spawnPoint, hit.position, timeToTarget);
        gem.DespawnAfter(droppedTreasureDespawnTime);
    }

    //Used to manually drop the treasure infront of the player
    public void DropTreasure()
    {
        if (collectedPickupable != null)
        {
            Vector3 position = transform.position;
            Vector3 dropPosition = transform.forward / (collectedPickupable.Worth > 1f ? throwPenalty : 1.5f);

            Vector3 spawnPosition = position + dropPosition;
            Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            Rigidbody rb = GetComponent<Rigidbody>();
            SetGem(collectedPickupable, spawnPoint);
            collectedPickupable.SetCollider(ColliderState.Flying, true);
            collectedPickupable.transform.SetParent(null, true);
            collectedPickupable.SetKinematic(false);
            collectedPickupable.parentInteration = this;
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
            animator.SetBool("IsHolding", true);
            animator.SetTrigger("Steal");
            gem.GetComponent<Collider>().enabled = false;
            collectedPickupable = gem;
        }
    }

    private void RemoveNearbyGem(PickupCollected collected)
    {
        if (nearbyPickable = collected.pickup)
        {
            nearbyPickable = null;
            EnableButtonIndicator(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<TreasureZone>() != null)
        {
            isInTreasureZone = true;
            if (collectedPickupable == null)
            {
                EnableButtonIndicator(true);
            }
        }

        if (other.TryGetComponent<Minecart>(out var minecart))
        {
            currentMinecart = minecart;
            if (collectedPickupable != null)
            {
                EnableButtonIndicator(true);
            }
            isInCollectionZone = true;
        }

        else if (other.GetComponent<Pickupable>() != null)
        {
            if (!IsPlayerStunned() && collectedPickupable == null)
            {
                EnableButtonIndicator(true);
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

        else if (other.GetComponent<CollectionZone>() != null)
        {
            Minecart minecart = other.GetComponent<Minecart>();
            if (minecart != null && minecart == currentMinecart)
            {
                currentMinecart = null;
            }

            isInCollectionZone = false;
            EnableButtonIndicator(false);
        }

        else if (other.GetComponent<Pickupable>())
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
        if (!state)
        {
            inputIndicator.gameObject.SetActive(false);
            inputIndicatorCooldown.gameObject.SetActive(false);
            return;
        }

        if (isInTreasureZone)
        {
            if (Time.time > pickUpCooldown)
            {
                inputIndicator.gameObject.SetActive(true);
                inputIndicatorCooldown.gameObject.SetActive(false);
            }
            else
            {
                inputIndicator.gameObject.SetActive(false);
                inputIndicatorCooldown.gameObject.SetActive(true);
            }
        }
        else
        {
            if (Time.time > pickUpCooldown)
            {
                inputIndicator.gameObject.SetActive(true);
            }
            else
            {
                inputIndicator.gameObject.SetActive(false);
                inputIndicatorCooldown.gameObject.SetActive(false);
            }
        }
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
        }
        else if (nearbyPickable != null && collectedPickupable == null)
        {
            CollectTreasureFromGround(nearbyPickable);
            animator.SetTrigger("PickUp");
            animator.SetBool("IsHolding", true);
            nearbyPickable = null;
            EnableButtonIndicator(false);
        }
        else if (isInCollectionZone && collectedPickupable != null && !currentMinecart.IsFull)
        {
            DeliverTreasure();
            isInCollectionZone = false;
            EnableButtonIndicator(false);
        }
        else if (collectedPickupable != null)
        {
            animator.SetTrigger("Throw");
            animator.SetBool("IsHolding", false);
        }
    }

    private void HandleTreasureDespawned(Pickupable pickupable)
    {
        pickupable.OnPickupableDespawnedEvent -= HandleTreasureDespawned;
        if (!isInCollectionZone || !isInTreasureZone)
        {
            EnableButtonIndicator(false);
        }
    }

    public void HandleTreasureEnteredMinecart(Pickupable pickupable, int value)
    {
        OnTreasureDelivered?.Invoke(value);
    }

    private void OnDisable()
    {
        EventBus<PickupCollected>.OnEvent -= RemoveNearbyGem;
    }
}
