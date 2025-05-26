using UnityEngine;
using UnityEngine.Events;

public class HubManager : MonoBehaviour
{
    [SerializeField] private UnityEvent OnStart;

    [SerializeField] private GameObject[] scoreDisplayObjects;
    [SerializeField] private GameObject[] hubAreaObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnStart?.Invoke();
        if (PlayerPrefs.GetInt("DoPodium", 0) == 1) SetUpScoreDisplay();
        else SetUpHubArea();
    }

    private void SetUpScoreDisplay()
    {
        foreach (var obj in scoreDisplayObjects) obj.SetActive(true);

        FindFirstObjectByType<PodiumController>().StartPodiumSequence();
    }

    private void SetUpHubArea()
    {
        foreach (var obj in hubAreaObjects) obj.SetActive(true);

        FindFirstObjectByType<PlayerDistributor>().InstantiatePlayersInCircle(1);
    }
}
