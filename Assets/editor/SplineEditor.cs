using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    private Spline spline;

    private const float handleSize = 0.1f;


    private void OnEnable()
    {
        spline = target as Spline;
    }

    private void OnSceneGUI()
    {
        Handles.BeginGUI();
        Rect editorView = SceneView.lastActiveSceneView.position;

        Rect button = new Rect(editorView.width - 85, editorView.height - 60, 80, 30);
        if (GUI.Button(button, "Add Point"))
        {
            AddPoint();
        }
        button.x -= 100;
        button.width += 20;
        if (GUI.Button(button, "Remove Last"))
        {
            spline.RemoveLastPoint();
        }
        button.x -= 100;
        if (GUI.Button(button, "Reset Curve"))
        {
            spline.ResetSpline();
        }
        Handles.EndGUI();

        if (spline.ControlPointCount < 3) return;

        Handles.color = Color.gray;
        for (int i = 0; i < spline.ControlPointCount - 3; i += 3)
        {
            Vector3 p0 = ShowPoint(i);
            Vector3 p1 = ShowPoint(i + 1);
            Vector3 p2 = ShowPoint(i + 2);
            Vector3 p3 = ShowPoint(i + 3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);

            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        spline = target as Spline;
    }
    void AddPoint()
    {
        Undo.RecordObject(spline, "Add Point");
        Vector3 point = spline.GetControlPoint(spline.ControlPointCount - 1);
        Vector3 direction = spline.GetControlPoint(spline.ControlPointCount - 1) - spline.GetControlPoint(spline.ControlPointCount - 2);
        direction.Normalize();
        spline.AddControlPoint(point + direction);
        spline.AddControlPoint(point + direction * 3);
        spline.AddControlPoint(point + direction * 6);
    }


    private Vector3 ShowPoint(int index)
    {
        Vector3 point = spline.GetControlPoint(index);    
        EditorGUI.BeginChangeCheck();
        point = CustomXYHandle.XYPlaneMoveHandle(point, handleSize);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            spline.SetControlPoint(index, point);
        }
        return point;
    }
}