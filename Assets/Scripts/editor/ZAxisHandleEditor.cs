using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ZAxisHandle))]
public class ZAxisHandleEditor : Editor
{
    private void OnSceneGUI()
    {
        ZAxisHandle zAxisHandle = (ZAxisHandle)target;
        Transform targetTransform = zAxisHandle.transform;

        EditorGUI.BeginChangeCheck();

        // Draw the Z axis handle
        Vector3 newPosition = Handles.Slider(targetTransform.position, targetTransform.forward, HandleUtility.GetHandleSize(targetTransform.position), Handles.ArrowHandleCap, 0.1f);

        if (EditorGUI.EndChangeCheck())
        {
            // Record the change for Undo functionality
            Undo.RecordObject(targetTransform, "Move Z Axis");
            targetTransform.position = newPosition;
        }
    }
}
