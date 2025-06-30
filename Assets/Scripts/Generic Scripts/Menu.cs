using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

        //Raise the scene restart event to clean up before reloading the scene
        EventBus<SceneRestart>.RaiseEvent(new());

        //Reset the chromatic abberation strength
        ResetChromaticAbberation();

        Scheduler.Instance.StopAllRoutines();
        SceneManager.LoadScene(scene.name);
    }

    public void GoToHub()
    {
        //Reset the chromatic abberation strength
        ResetChromaticAbberation();

        PlayerPrefs.SetInt("DoPodium", 0);

        Services.Get<PauseManager>().Unpause();
        SceneManager.LoadScene("HubScene");
    }

    private void ResetChromaticAbberation()
    {
        FindFirstObjectByType<Volume>().sharedProfile.TryGet<ChromaticAberration>(out var chromatic);
        chromatic.intensity.value = 0;
    }
}
