// Version 2023
//  (Update: student labture version, with TODO's)

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Curve))]
public class CurveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }

    // This method is called by Unity whenever it renders the scene view.
    // We use it to draw gizmos, and deal with changes (dragging objects)
    void OnSceneGUI()
    {
        serializedObject.Update();

        Curve curve = serializedObject.targetObject as Curve;

        if (curve.points == null)
            return;

        bool dirty = false;

        // Add new points if needed:
        Event e = Event.current;
        if ((e.type == EventType.KeyDown && e.keyCode == KeyCode.Space))
        {
            Debug.Log("Space pressed - trying to add point to curve");
            dirty |= AddPoint(curve);
        }

        dirty |= ShowAndMovePoints(curve);

        serializedObject.ApplyModifiedProperties();
    }

    // Tries to add a point to the curve, where the mouse is in the scene view.
    // Returns true if a change was made.
    bool AddPoint(Curve curve)
    {
        bool dirty = false;
        Transform handleTransform = curve.transform;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Adding spline point at mouse position: " + hit.point);
            Undo.RecordObject(target, "Adding spline");
            curve.points.Add(handleTransform.InverseTransformPoint(hit.point));
            EditorUtility.SetDirty(target);
            dirty = true;
        }
        return dirty;
    }

    // Show points in scene view, and check if they're changed:
    bool ShowAndMovePoints(Curve curve)
    {
        bool dirty = false;
        Transform handleTransform = curve.transform;

        Vector3 previousPoint = handleTransform.TransformPoint(curve.points[^1]);
        for (int i = 0; i < curve.points.Count; i++)
        {
            Vector3 currentPoint = handleTransform.TransformPoint(curve.points[i]);

            Debug.DrawLine(previousPoint, currentPoint, Color.white);

            previousPoint = currentPoint;

            EditorGUI.BeginChangeCheck();
            currentPoint = Handles.DoPositionHandle(currentPoint, ((Curve)target).transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Point transform change");
                curve.points[i] = handleTransform.InverseTransformPoint(currentPoint);
                EditorUtility.SetDirty(target);
                dirty = true;
            }

        }
        return dirty;
    }
}
