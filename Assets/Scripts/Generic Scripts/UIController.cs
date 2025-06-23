using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;

    void Start()
    {
        Services.Get<PauseManager>().OnPaused += HandleOnPaused;
    }

    private void HandleOnPaused(bool isPaused, int playerId)
    {
        if (pauseUI != null)
            pauseUI.SetActive(isPaused);
    }

    private void OnDestroy()
    {
        Services.Get<PauseManager>().OnPaused -= HandleOnPaused;
    }
}
