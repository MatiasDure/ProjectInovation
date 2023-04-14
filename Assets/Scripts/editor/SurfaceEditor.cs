using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

[CustomEditor(typeof(Surface))]
public class SurfaceEditor : Editor
{

    private Surface surface;

    private void OnEnable()
    {
        surface = (Surface)target;  
    }


    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        serializedObject.Update();
        surface.surfaceType = (Surface.SurfaceType)EditorGUILayout.EnumPopup(nameof(surface.surfaceType), surface.surfaceType);

        switch (surface.surfaceType)
        {
            case Surface.SurfaceType.Default:
                RemoveWindTrigger();
                //surface.SetColor(ColorsHolder.Instance.white);
                surface.gameObject.GetComponent<BoxCollider>().material = null;

                if(surface.windVisualPrefab != null) DestroyImmediate(surface.windVisualPrefab);
                break;

            case Surface.SurfaceType.Ice:
                RemoveWindTrigger();
                //surface.SetColor(ColorsHolder.Instance.blue);
                surface.gameObject.GetComponent<BoxCollider>().material = ColorsHolder.Instance.ice;

                if (surface.windVisualPrefab != null) DestroyImmediate(surface.windVisualPrefab);
                break;

            case Surface.SurfaceType.Sticky:
                RemoveWindTrigger();
                //surface.SetColor(ColorsHolder.Instance.yellow);
                surface.gameObject.GetComponent<BoxCollider>().material = null;

                if (surface.windVisualPrefab != null) DestroyImmediate(surface.windVisualPrefab);
                /*                surface.stickyDragForce = EditorGUILayout.FloatField(nameof(surface.stickyDragForce), surface.stickyDragForce);*/
                break;
            case Surface.SurfaceType.Bouncy:
                RemoveWindTrigger();
                //surface.SetColor(ColorsHolder.Instance.green);
                surface.gameObject.GetComponent<BoxCollider>().material = null;
                surface.bounceAmount = EditorGUILayout.FloatField(nameof(surface.bounceAmount), surface.bounceAmount);

                if (surface.windVisualPrefab != null) DestroyImmediate(surface.windVisualPrefab);
                break;
            case Surface.SurfaceType.Wind:
                //surface.SetColor(ColorsHolder.Instance.red);
                surface.gameObject.GetComponent<BoxCollider>().material = null;
                surface.windForce = EditorGUILayout.FloatField(nameof(surface.windForce), surface.windForce);
                surface.windTriggerHeight = EditorGUILayout.FloatField(nameof(surface.windTriggerHeight), surface.windTriggerHeight);
                if(surface.windVisualPrefab == null) surface.windVisualPrefab = Instantiate(ColorsHolder.Instance.windVisual, surface.transform);
                surface.UpdateWindTiggerSize();

                BoxCollider[] colliders = surface.gameObject.GetComponents<BoxCollider>();
                if(colliders.Length < 2)
                {
                    BoxCollider trigger = surface.gameObject.AddComponent<BoxCollider>();
                    trigger.isTrigger = true;
                    surface.windTrigger = trigger; 
                }
                else
                {
                    foreach (var collider in colliders)
                    {
                        if (collider.isTrigger) surface.windTrigger = collider;
                    }
                }
                break;
            case Surface.SurfaceType.Death:
                RemoveWindTrigger();
                surface.gameObject.GetComponent<BoxCollider>().material = null;

                if (surface.windVisualPrefab != null) DestroyImmediate(surface.windVisualPrefab);
                break;

            default:
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    void RemoveWindTrigger()
    {
        BoxCollider[] colliders = surface.gameObject.GetComponents<BoxCollider>();

        if(colliders.Length > 1)
        {
            foreach (var collider in colliders)
            {
                if (collider.isTrigger)
                {
                    DestroyImmediate(collider);
                    return;
                }
            }
        }
    }


}
