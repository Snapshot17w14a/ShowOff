using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Continue()
    {
        ServiceLocator.GetService<PauseManager>().SetIsPaused();
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        ServiceLocator.GetService<PauseManager>().SetIsPaused();
    }

    public void GoToHub()
    {
        SceneManager.LoadScene("HubScene");
        ServiceLocator.GetService<PauseManager>().SetIsPaused();
    }
}
