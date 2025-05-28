using UnityEngine;

public class IdleTimer : MonoBehaviour
{
    [SerializeField] private MinigameState idleState;
    [SerializeField] private MinigameHandler handler;

    [SerializeField] private float maxIdleTime = 30;
    private float idleTime;

    void Update()
    {
        idleTime += Time.deltaTime;

        if (Input.anyKey) idleTime = 0;

        if (idleTime >= maxIdleTime)
        {
            idleTime = 0;
            handler.LoadState(idleState);
        }
    }
}
