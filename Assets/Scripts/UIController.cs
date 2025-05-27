using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    void Start()
    {
        ServiceLocator.GetService<PauseManager>().OnPaused += HandleOnPaused; 
    }

    private void HandleOnPaused(bool isPaused)
    {
        pauseUI.gameObject.SetActive(isPaused);
    }

    private void OnDestroy()
    {
        ServiceLocator.GetService<PauseManager>().OnPaused -= HandleOnPaused;
    }
}
