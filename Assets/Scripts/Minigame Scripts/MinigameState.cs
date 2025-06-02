using UnityEngine.Events;
using UnityEngine;

public abstract class MinigameState : MonoBehaviour
{
    [Header("Time and State")]
    [SerializeField] protected MinigameState nextMinigameState;
    [SerializeField] protected float stateDurationSeconds;

    [Header("Events")]
    [SerializeField] protected UnityEvent onStateLoaded;
    [SerializeField] protected UnityEvent onStateUnloaded;
    [SerializeField] protected UnityEvent onStateTick;

    public virtual void LoadState()
    {
        gameObject.SetActive(true);
        onStateLoaded?.Invoke();
        if (stateDurationSeconds != 0 && nextMinigameState != null) StartCoroutine(FindFirstObjectByType<MinigameHandler>().ChangeStateInSeconds(stateDurationSeconds, nextMinigameState));
    }

    public virtual void UnloadState()
    {
        gameObject.SetActive(false);
        onStateUnloaded?.Invoke();
    }

    public virtual void TickState()
    {
        onStateTick?.Invoke();
    }
}
