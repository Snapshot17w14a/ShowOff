using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Continue()
    {
        ServiceLocator.GetService<PauseManager>().Unpause();
    }

    public void Restart()
    {
        ServiceLocator.GetService<PauseManager>().Unpause();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void GoToHub()
    {
        ServiceLocator.GetService<PauseManager>().Unpause();
        SceneManager.LoadScene("HubScene");
    }
}
