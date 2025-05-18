using UnityEngine;

public class Minecart : MonoBehaviour
{
    [SerializeField] private GameObject movePoint;
    [SerializeField] private int maxGemIntake = 3;
    [SerializeField] private float speed = 5f;

    private int currentGemAmount;
    private Vector3 startPosition;
    private Vector3 endPosition;

    void Start()
    {
        startPosition = transform.position;
        endPosition = movePoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if(IsCartFull())
        {
            Vector3 direction = endPosition - startPosition;
            Vector3 dirNormalized = direction.normalized;
            transform.position = direction * speed;

            if(startPosition == endPosition)
            {

            }
        }
    }

    private void WaitAndReturn()
    {

    }

    private bool IsCartFull()
    {
        return currentGemAmount >= maxGemIntake;
    }
}
