using UnityEditor;
using UnityEngine;
using static BobAttackPattern;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(BobAttackPattern))]
public class BobAttackPatternEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var attackContainerList = serializedObject.FindProperty("bobAttackContainers");

        BeginHorizontal();

        attackContainerList.arraySize = Mathf.Max(0, IntField("Arr size", attackContainerList.arraySize));

        if (GUILayout.Button(new GUIContent("+"))) attackContainerList.arraySize++;
        if (GUILayout.Button(new GUIContent("-"))) attackContainerList.arraySize -= attackContainerList.arraySize != 0 ? 1 : 0;

        EndHorizontal();

        PropertyField(attackContainerList);

        //for (int i = 0; i < attackContainerList.arraySize; i++)
        //{
        //    DrawArrayElement(attackContainerList.GetArrayElementAtIndex(i));
        //}

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawArrayElement(SerializedProperty element)
    {
        BeginVertical("Box");
        PropertyField(element);
        EndVertical();
    }
}
