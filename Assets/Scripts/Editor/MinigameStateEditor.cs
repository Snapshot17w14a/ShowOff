using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MinigameState), true)]
public class MinigameStateEditor : Editor
{
    private static Object lastObject;

    private bool isEnabled = true;

    private void OnEnable()
    {
        if (!isEnabled || Application.isPlaying || lastObject == target) return;
        Debug.Log("Selecting: " + target);
        var currentTransform = (target as MinigameState).transform;
        foreach (Transform sibling in currentTransform.parent) sibling.gameObject.SetActive(false);
        currentTransform.gameObject.SetActive(true);
        isEnabled = true;
        lastObject = target;
    }
}
