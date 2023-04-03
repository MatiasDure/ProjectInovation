using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckPointManager))]
public class CheckPointEditor : Editor
{

    private CheckPointManager checkPointManager;

    List<CheckPoint> points = new List<CheckPoint>();

    private void OnEnable()
    {
        checkPointManager = (CheckPointManager)target;
        points = checkPointManager.checkPointPositions;

        Tool LastTool = Tools.current;
        Tools.current = Tool.None;
        
    }


    void DrawCubes()
    {
        foreach (var point in points)
        {

        }
    }

    void OnDrawGizmos()
    {

        Handles.BeginGUI();


        Rect editorView = SceneView.lastActiveSceneView.position; 

        if (GUI.Button(new Rect(editorView.width - 110, editorView.height - 80, 100, 50), "Add Checkpoint"))
        {
            Vector3 position = GetMouseWorldPositionOnXYPlane(new Vector2(editorView.width / 2, editorView.height / 2));
            AddPoint(position, points.Count);
        }


        Handles.EndGUI();
    }

    public static Vector3 GetMouseWorldPositionOnXYPlane(Vector2 poisiton)
    {
        Camera editorCamera = SceneView.lastActiveSceneView.camera;
        Vector2 mousePosition = poisiton;

        // Convert the mouse position from GUI space to screen space
        float screenHeight = editorCamera.pixelHeight;
        mousePosition.y = screenHeight - mousePosition.y;

        // Calculate the world position using the camera
        Ray ray = editorCamera.ScreenPointToRay(mousePosition);

        // Calculate the intersection point with the X-Y plane
        float distance;
        Plane xyPlane = new Plane(-Vector3.forward, Vector3.zero);
        if (xyPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }



    void OnSceneGUI()
    {

        OnDrawGizmos();

        DrawAndMoveCurve();
        DrawCubes();

    }

    void DrawAndMoveCurve()
    {
        for (int i =0; i< points.Count; i++)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 16; // Set the desired font size

            Handles.Label(points[i].position + new Vector2(points[i].size.x/2, points[i].size.y), $"{points[i].index}", labelStyle);

            Color ogColor = Handles.color;
            Handles.color = Color.red;

            if (Handles.Button(points[i].position + new Vector2(0, points[i].size.y/2), Quaternion.identity, 0.2f, 0.2f, CustomXYHandle.SolidCircleCap))
            {
                points.Remove(points[i]);
                SortCheckpoints();
                SaveValues();
                return;
            }

            Handles.color = ogColor;



            EditorGUI.BeginChangeCheck();
            Vector2 newPosition = CustomXYHandle.XYPlaneMoveHandle(points[i].position, 0.1f);

            Vector3 newScale = Handles.Slider(newPosition + new Vector2(0,points[i].size.y), Vector3.up, 1, CustomArrowCap.DrawCap, 0.01f);
            float yScale = newScale.y - newPosition.y; 

            if (EditorGUI.EndChangeCheck())
            {
                Ray ray = new Ray(newPosition,Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit,1))
                {
                    newPosition = hit.point;
                }


                Undo.RecordObject(checkPointManager.transform, "Move curve");
                EditorUtility.SetDirty(checkPointManager);
                points[i].position = newPosition;
                points[i].size = new Vector2(points[i].size.x, yScale);
                //checkPointManager.checkPointPositions = points;
            }
        }
        SaveValues(); 
    }

    void SaveValues()
    {
        for (int i = 0; i < points.Count; i++)
        {
            checkPointManager.checkPointPositions = points;
        }
    }
    void SortCheckpoints()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].index = i; 
        }
    }

    void AddPoint(Vector3 position ,int index)
    {

        points.Add(new CheckPoint(position, new Vector2(1,3), index));
    }
}
