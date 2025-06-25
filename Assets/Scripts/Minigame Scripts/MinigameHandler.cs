using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MinigameHandler : MonoBehaviour
{
    [Header("Game states")]
    [SerializeField] private MinigameState initialMinigameState;

    [Header("Events")]
    [SerializeField] private UnityEvent onStateChanged;

    private MinigameState currentMinigameState;
    private MinigameState nextMinigameState;

    public static MinigameHandler Instance { get; private set; }

    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (Transform child in transform) child.gameObject.SetActive(false);

        LoadState(initialMinigameState);
    }

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
        if (currentMinigameState != null) currentMinigameState.UnloadState();
        currentMinigameState = state;
        currentMinigameState.LoadState();
        onStateChanged?.Invoke();
    }

    public void SpecialUnload(Func<MinigameState, MinigameState> unloadFunction) => currentMinigameState = unloadFunction(currentMinigameState);

    public void InstantiateAllPlayers() => Services.Get<PlayerRegistry>().InstantiateAllPlayers();

    public void WipeScoreData() => Services.Get<ScoreRegistry>().WipeData();

    private void OnDestroy() => Instance = null;

    public void SetAutoJoinStatus(bool status)
    {
        Services.Get<PlayerAutoJoin>().AllowJoining = status;
    }
}
