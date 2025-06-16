using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;

    void Start()
    {
        ServiceLocator.GetService<PauseManager>().OnPaused += HandleOnPaused;
    }

    private void HandleOnPaused(bool isPaused, int playerId)
    {
        if (pauseUI != null)
            pauseUI.SetActive(isPaused);
    }

    private void OnDestroy()
    {
        ServiceLocator.GetService<PauseManager>().OnPaused -= HandleOnPaused;
    }
}
