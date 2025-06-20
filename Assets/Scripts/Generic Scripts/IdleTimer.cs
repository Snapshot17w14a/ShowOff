using UnityEngine;

public class IdleTimer : MonoBehaviour
{
    [SerializeField] private MinigameState idleState;
    [SerializeField] private MinigameHandler handler;

    [SerializeField] private float maxIdleTime = 30;
    private float idleTime;
    private bool isTransitioning = false;

    void Update()
    {
        idleTime += Time.deltaTime;

        if (Input.anyKey) idleTime = 0;

        if (idleTime >= maxIdleTime && !isTransitioning)
        {
            isTransitioning = true;
            TransitionController.Instance.TransitionOut(1f, () =>
            {
                idleTime = 0;
                isTransitioning = false;
                handler.LoadState(idleState);
                TransitionController.Instance.TransitionIn(1f);
            });
        }
    }
}
