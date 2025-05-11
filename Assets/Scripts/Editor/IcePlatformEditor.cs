using static UnityEditor.EditorGUILayout;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(IcePlatformManager))]
public class IcePlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var minSecondsProperty = serializedObject.FindProperty("minSeconds");
        var maxSecondsProperty = serializedObject.FindProperty("maxSeconds");

        var minSeconds = minSecondsProperty.floatValue;
        var maxSeconds = maxSecondsProperty.floatValue;

        BeginHorizontal();

        LabelField("Ice brittleness time", GUILayout.Width(150));
        LabelField(MathF.Round(minSeconds, 1).ToString(), GUILayout.Width(25));
        MinMaxSlider(ref minSeconds, ref maxSeconds, 0, 15);
        LabelField(MathF.Round(maxSeconds, 1).ToString(), GUILayout.Width(25));

        EndHorizontal();

        minSecondsProperty.floatValue = MathF.Round(minSeconds, 1);
        maxSecondsProperty.floatValue = MathF.Round(maxSeconds, 1);

        serializedObject.ApplyModifiedProperties();
    }
}
