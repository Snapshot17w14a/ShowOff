using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    [SerializeField] private float lerpSpeed;

    private Image image;

    private static TransitionController instance;
    public static TransitionController Instance => instance;

    private Guid routineGuid;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        image = transform.GetChild(0).GetComponent<Image>();
        SceneManager.sceneLoaded += TransitionIn;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
        SceneManager.sceneLoaded -= TransitionIn;
    }

    public void TransitionOut(string sceneName, float duration = 1)
    {
        if (routineGuid != Guid.Empty) Scheduler.Instance.StopRoutine(routineGuid);
        routineGuid = Scheduler.Instance.Lerp(LerpAlpha, duration, () =>
        {
            SceneManager.LoadScene(sceneName);
            routineGuid = Guid.Empty;
        });
    }

    public void TransitionIn(Scene scene, LoadSceneMode sceneLoadMode)
    {
        if (routineGuid != Guid.Empty) Scheduler.Instance.StopRoutine(routineGuid);
        Scheduler.Instance.Lerp(t => LerpAlpha(1 - t), 1f, () => routineGuid = Guid.Empty);
    }

    public void TransitionOut(float duration = 1, Action callback = null)
    {
        if (routineGuid != Guid.Empty) Scheduler.Instance.StopRoutine(routineGuid);
        routineGuid = Scheduler.Instance.Lerp(LerpAlpha, duration, () =>
        {
            routineGuid = Guid.Empty;
            callback?.Invoke();
        });
    }

    public void TransitionIn(float duration = 1, Action callback = null)
    {
        if (routineGuid != Guid.Empty) Scheduler.Instance.StopRoutine(routineGuid);
        Scheduler.Instance.Lerp(t => LerpAlpha(1 - t), duration, () => { routineGuid = Guid.Empty; callback?.Invoke(); });
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LerpAlpha(float t)
    {
        image.color = new Color(0, 0, 0, t);
    }
}