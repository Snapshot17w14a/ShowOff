using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class MinigamePlayer : MonoBehaviour
{
    public TreasureInteraction TreasureInteraction { get; private set; }

    public event Action OnPlayerStunned;

    public bool IsStunned => isStunned;

    private Vector2 inputVector = Vector2.zero;
    private new Rigidbody rigidbody;
    private VisualEffect stunEffect;
    private MeshRenderer meshRenderer;

    [Header("View settings")]
    [SerializeField] private float turnSpeedMultiplier;
    [SerializeField] private float movementSpeed = 1.0f;

    [Header("Stun settings")]
    [SerializeField] private float blinkInterval = 0.33f;

    [Header("Dash settings")]
    [SerializeField] private float dashStunDuration = 1f;
    [SerializeField] private float dashCooldown = 5f;
    [SerializeField] private float dashDuration = 5f;
    [SerializeField] private float dashForce = 5f;
    [SerializeField] private GameObject dashIndicator;
    private Material dashIndicatorMaterial;

    [Header("Color changes")]
    [SerializeField] private SpriteRenderer[] spritesToRecolor;
    [SerializeField] private VisualEffect walkEffect;

    private bool isDashAvailable = true;
    private float dashTimer = 0f;

    private bool isStunned = false;
    private bool isFlying = false;
    private bool isDashing = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        stunEffect = GetComponentInChildren<VisualEffect>();
        TreasureInteraction = GetComponent<TreasureInteraction>();
        stunEffect.Stop();

        dashIndicatorMaterial = dashIndicator.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateDashTimer();
        if (isStunned || isFlying) return;
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (inputVector.sqrMagnitude == 0) return;
        Vector3 inputDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        rigidbody.AddForce(movementSpeed * Time.deltaTime * inputDirection);
        var targetAngle = Vector3.SignedAngle(Vector3.forward, inputDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * turnSpeedMultiplier);
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    private void OnDash()
    {
        if (isDashAvailable && !isStunned && !isFlying) Dash();
    }

    public void StunPlayer(float seconds)
    {
        TreasureInteraction.DropTreasureRandom();
        StartCoroutine(StunRoutine(seconds));
        OnPlayerStunned?.Invoke();
    }

    public void SetFlightState(bool state) => isFlying = state;

    public void SetPlayerColor(Color color, int playerId)
    {
        foreach (var renderer in spritesToRecolor) renderer.color = color;

        GetComponent<MeshRenderer>().material.color = color;

        var textMeshPro = GetComponentInChildren<TextMeshPro>();
        textMeshPro.color = color;
        textMeshPro.text = $"P{playerId + 1}";

        dashIndicator.GetComponent<MeshRenderer>().material.SetColor("_ColorCircle", color);
    }

    private void Dash()
    {
        //Set flags and reset the timer
        isDashing = true;
        isDashAvailable = false;
        dashTimer = 0f;

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
        stunEffect.Play();
        rigidbody.linearVelocity = Vector3.zero;

        float timer = 0;
        float nextBlinkTime = blinkInterval;

        while (timer < stunSeconds)
        {
            timer += Time.deltaTime;
            if (timer >= nextBlinkTime)
            {
                nextBlinkTime += blinkInterval;
                var currentColor = meshRenderer.material.color;
                currentColor.a = currentColor.a == 0.5f ? 1f : 0.5f;

                meshRenderer.material.color = currentColor;
            }
            yield return new WaitForEndOfFrame();
        }

        isStunned = false;
        stunEffect.Stop();
        var solidColor = meshRenderer.material.color;
        solidColor.a = 1f;
        meshRenderer.material.color = solidColor;

        yield return null;
    }

    private IEnumerator ResetDashInSeconds(float dashDuration)
    {
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDashing || !collision.gameObject.CompareTag("Player")) return;

        collision.gameObject.GetComponent<MinigamePlayer>().StunPlayer(dashStunDuration);
    }
}
