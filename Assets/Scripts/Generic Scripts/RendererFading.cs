using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererFading : MonoBehaviour
{
    private List<MeshRenderer> renderers = new();

    //Only for the TryGetComponent to use
    private MeshRenderer tryRenderer;

    void Start()
    {
        GetRenderersRecursively(transform);
    }

    private void GetRenderersRecursively(Transform transform)
    {
        if (transform.childCount == 0) return;

        foreach (Transform child in transform)
        {
            child.TryGetComponent(out tryRenderer);
            if (tryRenderer != null && !tryRenderer.name.Contains("Text")) renderers.Add(tryRenderer);

            GetRenderersRecursively(child);
        }
    }

    public void FadeAllChildOut(float fadeSeconds)
    {
        if (renderers.Count == 0) GetRenderersRecursively(transform);
        StartCoroutine(FadeOut(fadeSeconds, renderers.ToArray()));
    }

    public void FadeAllChildOut(float fadeSeconds, float delay)
    {
        if (renderers.Count == 0) GetRenderersRecursively(transform);
        StartCoroutine(DelayExecution(FadeOut(fadeSeconds, renderers.ToArray()), delay));
    }

    private IEnumerator FadeOut(float seconds, Renderer[] renderers)
    {
        var time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / seconds;

            foreach (var renderer in renderers)
            {
                var rendererColor = renderer.material.color;
                rendererColor.a = Mathf.Lerp(1, 0, time);
                renderer.material.color = rendererColor;
            }

            yield return null;
        }
    }

    private IEnumerator DelayExecution(IEnumerator routine, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(routine);
    }
}
