using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private PauseManager pauseManager;
    void Awake()
    {
        pauseManager = ServiceLocator.GetService<PauseManager>();
    }

    public void Continue()
    {

    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        pauseManager.SetIsPaused();
    }

    public void GoToHub()
    {
        SceneManager.LoadScene("HubScene");
        pauseManager.SetIsPaused();
    }
}
