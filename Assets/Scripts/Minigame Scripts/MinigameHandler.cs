using UnityEngine.Events;
using System.Collections;
using UnityEngine;

public class MinigameHandler : MonoBehaviour
{
    [Header("Game states")]
    [SerializeField] private MinigameState initialMinigameState;

    [Header("Events")]
    [SerializeField] private UnityEvent onStateChanged;

    private MinigameState currentMinigameState;
    private MinigameState nextMinigameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMinigameState = initialMinigameState;
        currentMinigameState.LoadState();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMinigameState != null) currentMinigameState.TickState();
    }

    public IEnumerator ChangeStateInSeconds(float seconds, MinigameState state)
    {
        nextMinigameState = state;
        yield return new WaitForSeconds(seconds);
        currentMinigameState.UnloadState();
        currentMinigameState = state;
        currentMinigameState.LoadState();
        onStateChanged?.Invoke();
    }

    public void LoadNextScene()
    {
        if (nextMinigameState == null)
        {
            Debug.LogError("Next scene reference was null");
            return;
        }
        StopAllCoroutines();
        currentMinigameState.UnloadState();
        currentMinigameState = nextMinigameState;
        nextMinigameState = null;
        currentMinigameState.LoadState();
        onStateChanged?.Invoke();
    }

    public void InstantiateAllPlayers() => ServiceLocator.GetService<PlayerRegistry>().InstantiateAllPlayers();
}
