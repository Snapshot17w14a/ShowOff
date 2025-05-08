using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class MinigamePlayer : MonoBehaviour
{
    private Vector2 inputVector = Vector2.zero;
    private new Rigidbody rigidbody;

    [SerializeField] private float movementSpeed = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        rigidbody.AddForce(movementSpeed * Time.deltaTime * new Vector3(inputVector.x, 0, inputVector.y));
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }
}
