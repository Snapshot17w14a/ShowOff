using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class MinigamePlayer : MonoBehaviour
{
    public event Action<int> OnPlayerPaused;
    public TreasureInteraction TreasureInteraction { get; private set; }

    public bool IsStunned => isStunned;

    private Vector2 inputVector = Vector2.zero;
    private new Rigidbody rigidbody;

    [Header("Move settings")]
    [SerializeField] private float turnSpeedMultiplier;
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float walkEffectRate = 10f;

    [Header("Dash settings")]
    [SerializeField] private float dashStunDuration = 1f;
    [SerializeField] private float dashCooldown = 5f;
    [SerializeField] private float dashDuration = 5f;
    [SerializeField] private float dashForce = 5f;
    [SerializeField] private GameObject dashIndicator;
    private Material dashIndicatorMaterial;

    [Header("Color changes")]
    [SerializeField] private VisualEffect walkEffect;
    [SerializeField] private VisualEffect dashEffect;
    [SerializeField] private VisualEffect stunEffect;
    [SerializeField] private VisualEffect cryEffect;

    public bool IsCrying
    {
        set
        {
            if (value)
                cryEffect.Play();
            else
                cryEffect.Stop();
        }
    }


    public Animator GetPlayerAnimator => animator;
    [SerializeField] private Animator animator;

    private bool isDashAvailable = true;
    private float dashTimer = 0f;

    [SerializeField] private float smoothingMultiplier;
    private float smootedMoveVector = 0;

    private bool isStunned = false;
    private bool isFlying = false;
    private bool isDashing = false;

    public Color playerColor;

    /// <summary>
    /// The id of the player in the PlayerRegistry
    /// </summary>
    public int RegistryID { get; set; }

    private PlayerInput playerInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        TreasureInteraction = GetComponent<TreasureInteraction>();
        playerInput = GetComponent<PlayerInput>();

        dashIndicatorMaterial = dashIndicator.GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        Services.Get<ScoreRegistry>().AddPlayer(RegistryID);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateDashTimer();
        UpdateSmoothednputVector(inputVector.magnitude);
        animator.SetFloat("Magnitude", smootedMoveVector);
        if (isStunned || isFlying) return;
        UpdateMovement();
        walkEffect.SetFloat("Rate", rigidbody.linearVelocity.magnitude > 0.3 ? walkEffectRate : 0);
    }

    private void UpdateSmoothednputVector(float magnitude)
    {
        smootedMoveVector = Mathf.Lerp(smootedMoveVector, magnitude, Time.deltaTime * smoothingMultiplier);
    }

    private void UpdateMovement()
    {
        if (inputVector.sqrMagnitude == 0) return;

        Vector3 inputDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        float movementPenalty = Mathf.Clamp01(1f - (TreasureInteraction.MovementSpeedPenalty / 100f));

        rigidbody.AddForce((movementSpeed * (TreasureInteraction.IsHoldingItem && TreasureInteraction.CollectedPickupable.Worth > 1 ?
            movementPenalty : 1f)) * Time.deltaTime * inputDirection);

        var targetAngle = Vector3.SignedAngle(Vector3.forward, inputDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * turnSpeedMultiplier);
    }

    public void PushPlayer(int force)
    {
        Vector3 direction = transform.position - new Vector3(0, 1, 0);
        Vector3 dirNormalized = direction.normalized;
        rigidbody.AddForce(force * Time.deltaTime * dirNormalized, ForceMode.Impulse);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    public void OnDash()
    {
        if (isDashAvailable && !isStunned && !isFlying)
        {
            Dash();
            animator.SetTrigger("Dash");
        }
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Services.Get<PauseManager>().TogglePause(RegistryID);
        OnPlayerPaused?.Invoke(RegistryID);
    }

    public void SetInputEnabled(bool isEnabled)
    {
        if (isEnabled)
        {
            playerInput.ActivateInput();
        }
        else
        {
            playerInput.DeactivateInput();
        }
    }

    public void StunPlayer(float seconds)
    {
        StartCoroutine(StunRoutine(seconds));
    }

    public void SetFlightState(bool state) => isFlying = state;

    private void Dash()
    {
        //Set flags and reset the timer
        isDashing = true;
        isDashAvailable = false;
        dashTimer = 0f;

        dashEffect.Play();

        Vector3 forceVector = inputVector.sqrMagnitude == 0 ? transform.forward : new Vector3(inputVector.x, 0.5f, inputVector.y);
        rigidbody.AddForce(forceVector.normalized * dashForce, ForceMode.Impulse);
        StartCoroutine(ResetDashInSeconds(dashDuration));
    }

    private void UpdateDashTimer()
    {
        if (isDashAvailable) return;

        dashTimer += Time.deltaTime;
        dashIndicatorMaterial.SetFloat("_FillAmount", dashTimer / dashCooldown);
        if (dashTimer >= dashCooldown)
        {
            isDashAvailable = true;
            dashTimer = dashCooldown;
        }
    }

    private IEnumerator StunRoutine(float stunSeconds)
    {
        isStunned = true;
        animator.SetBool("IsHolding", false);
        animator.SetTrigger("Stun");
        stunEffect.Play();
        rigidbody.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(stunSeconds);

        isStunned = false;
        animator.SetTrigger("StunOver");
        stunEffect.Stop();
    }

    private IEnumerator ResetDashInSeconds(float dashDuration)
    {
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DeathBarrier"))
        {
            var pos = transform.position;
            pos.y = 1;
            transform.position = pos;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isDashing || !collision.gameObject.CompareTag("Player")) return;

        MinigamePlayer otherPlayer = collision.gameObject.GetComponent<MinigamePlayer>();

        if (otherPlayer != null && otherPlayer != this)
        {
            otherPlayer.StunPlayer(dashStunDuration);
            AudioManager.PlaySound(ESoundType.Penguin, "Player_Hit", false, 1f, 0.5f);
            TreasureInteraction otherTreasure = otherPlayer.TreasureInteraction;

            if (otherTreasure != null && otherTreasure.IsHoldingItem && this.TreasureInteraction != null && this.TreasureInteraction.IsHoldingItem)
            {
                otherTreasure.DropTreasureRandom();
            }
            else if (otherTreasure != null && otherTreasure.IsHoldingItem)
            {
                Pickupable pickUp = otherTreasure.CollectedPickupable;
                otherTreasure.CollectedPickupable.transform.SetParent(null, true);
                TreasureInteraction.CollectTreasureDirect(pickUp.PickupType, pickUp);
                otherTreasure.DropTreasureInstant();
            }
        }
    }
}
