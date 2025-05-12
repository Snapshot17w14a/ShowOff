using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadSceneWithName(string sceneName) => SceneManager.LoadScene(sceneName);
}
