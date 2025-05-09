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

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public virtual void LoadState()
    {
        onStateLoaded?.Invoke();
        gameObject.SetActive(true);
        StartCoroutine(FindFirstObjectByType<MinigameHandler>().ChangeStateInSeconds(stateDurationSeconds, nextMinigameState));
    }

    public virtual void UnloadState()
    {
        onStateUnloaded?.Invoke();
        gameObject.SetActive(false);
    }

    public virtual void TickState()
    {
        onStateTick?.Invoke();
    }
}
