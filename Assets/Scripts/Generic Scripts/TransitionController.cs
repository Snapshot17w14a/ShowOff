using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    [SerializeField] private float lerpSpeed;

    private string sceneToLoad;
    private float lerpValue = 1;
    private Image image;

    private static TransitionController instance;
    public static TransitionController Instance => instance;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        image = transform.GetChild(0).GetComponent<Image>();
        SceneManager.sceneLoaded += TransitionIn;
    }

    public void TransitionOut(string sceneName)
    {
        if (this == null || image == null) return;
        sceneToLoad = sceneName;
        StopAllCoroutines();
        StartCoroutine(Transition(true));
    }

    public void TransitionIn(Scene scene, LoadSceneMode sceneLoadMode)
    {
        if (this == null || image == null) return;
        StopAllCoroutines();
        StartCoroutine(Transition(false));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator Transition(bool isOutTransition)
    {
        Color imageColor = image.color;
        lerpValue = isOutTransition ? 0f : 1f;

        while (lerpValue >= 0 && lerpValue <= 1)
        {
            imageColor.a = Mathf.Clamp01(Mathf.Lerp(0f, 1f, lerpValue));
            image.color = imageColor;
            lerpValue += isOutTransition ? Time.deltaTime * lerpSpeed : -(Time.deltaTime * lerpSpeed);
            yield return new WaitForEndOfFrame();
        }

        if (isOutTransition) SceneManager.LoadScene(sceneToLoad);
        yield return null;
    }
}