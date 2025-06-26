using UnityEngine.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void Continue()
    {
        Services.Get<PauseManager>().Unpause();
    }
    public void Restart()
    {
        Services.Get<PauseManager>().Unpause();
        Scene scene = SceneManager.GetActiveScene();
        EventBus<SceneRestart>.RaiseEvent(new());
        Scheduler.Instance.StopAllRoutines();
        SceneManager.LoadScene(scene.name);
    }

    public void GoToHub()
    {
        Services.Get<PauseManager>().Unpause();
        SceneManager.LoadScene("HubScene");
    }
}
