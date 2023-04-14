using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdaptWind))]
public class AdaptWindEditor : Editor
{
    private AdaptWind adapter;
    // Start is called before the first frame update
    private void OnEnable()
    {
        adapter = (AdaptWind)target;
    }

    private void OnSceneGUI()
    {
        Handles.BeginGUI();
        Rect editorView = SceneView.lastActiveSceneView.position;

        Rect button = new Rect(editorView.width - 85, editorView.height - 60, 80, 30);
        if (GUI.Button(button, "Adapt"))
        {
            adapter.AdaptToObject(adapter.transform.parent);
        }
        Handles.EndGUI();
    }
}
