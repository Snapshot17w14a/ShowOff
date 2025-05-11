using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class MinigamePlayer : MonoBehaviour
{
    private Vector2 inputVector = Vector2.zero;
    private new Rigidbody rigidbody;

    [SerializeField] private float turnSpeedMultiplier;
    [SerializeField] private float movementSpeed = 1.0f;

    private float stunDuration = 0;
    private float stunTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HandleStun()) return;
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

    private bool HandleStun()
    {
        stunTime += Time.deltaTime;
        if (stunTime >= stunDuration)
        {
            stunDuration = 0;
            stunTime = 0;
            return false;
        }
        return true;
    }

    public void StunPlayer(float seconds)
    {
        stunDuration = seconds;
        rigidbody.linearVelocity = Vector3.zero;
    }
}
