using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class MinigamePlayer : MonoBehaviour
{
    private Vector2 inputVector = Vector2.zero;
    private new Rigidbody rigidbody;
    private VisualEffect stunEffect;
    private MeshRenderer meshRenderer;

    [SerializeField] private float turnSpeedMultiplier;
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float blinkInterval = 0.33f;

    private bool isStunned = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        stunEffect = GetComponentInChildren<VisualEffect>();
        stunEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) StunPlayer(2f);    
        if (isStunned) return;
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (inputVector.sqrMagnitude == 0) return;
        rigidbody.AddForce(movementSpeed * Time.deltaTime * new Vector3(inputVector.x, 0, inputVector.y));
        var targetAngle = Vector3.SignedAngle(Vector3.forward, rigidbody.linearVelocity.normalized, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * turnSpeedMultiplier);
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void StunPlayer(float seconds)
    {
        StartCoroutine(StunRoutine(seconds));
    }

    public void SetPlayerColor(Color color, int playerId)
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in spriteRenderers) renderer.color = color;

        var textMeshPro = GetComponentInChildren<TextMeshPro>();
        textMeshPro.color = color;
        textMeshPro.text = $"P{playerId + 1}";
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
}
