using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUI;
using static UnityEditor.EditorGUIUtility;

[CustomPropertyDrawer(typeof(BobAttackContainer))]
public class BobAttackContainerDrawer : PropertyDrawer
{
    private readonly Dictionary<string, int> nextLineCount = new();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        BeginProperty(position, label, property);

        var propertyPath = property.propertyPath;
        if (!nextLineCount.ContainsKey(propertyPath))
        {
            nextLineCount[propertyPath] = 0;
        }
        else nextLineCount[propertyPath] = 0;

        var rectPosition = new Rect(position.x, position.y, position.width, singleLineHeight);

        var state = property.FindPropertyRelative("State");
        PropertyField(rectPosition, state, new GUIContent("Attack"));
        NextLine(ref rectPosition, propertyPath);

        if (state.enumValueIndex == 0)
        {
            var time = property.FindPropertyRelative("time");
            time.floatValue = FloatField(rectPosition, "Idle time", time.floatValue);
            NextLine(ref rectPosition, propertyPath);
        }
    }

    private void NextLine(ref Rect rect, string path)
    {
        rect.y += singleLineHeight + standardVerticalSpacing;
        nextLineCount[path]++;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int multiplier = nextLineCount.ContainsKey(property.propertyPath) ? nextLineCount[property.propertyPath] : 1;
        return singleLineHeight * multiplier + standardVerticalSpacing * (multiplier - 1);
    }
}