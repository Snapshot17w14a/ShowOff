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

    public static MinigameHandler Instance => _instance;
    private static MinigameHandler _instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);

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
        LoadState(state);
    }

    public void LoadNextScene()
    {
        if (nextMinigameState == null)
        {
            Debug.LogError("Next state reference was null");
            return;
        }
        LoadState(nextMinigameState);
        nextMinigameState = null;
    }

    public void LoadState(MinigameState state)
    {
        StopAllCoroutines();
        currentMinigameState.UnloadState();
        currentMinigameState = state;
        currentMinigameState.LoadState();
        onStateChanged?.Invoke();
    }

    public void InstantiateAllPlayers() => ServiceLocator.GetService<PlayerRegistry>().InstantiateAllPlayers();

    public void WipeScoreData() => ServiceLocator.GetService<ScoreRegistry>().WipeData();

    private void OnDestroy() => _instance = null;
}
