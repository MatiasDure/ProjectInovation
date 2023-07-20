using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{

    [System.Serializable]
    public enum SurfaceType
    {
        Default,
        Ice,
        Sticky,
        Bouncy,
        Wind,
        Death
    }

    [HideInInspector] public SurfaceType surfaceType = SurfaceType.Default;

    [HideInInspector] public float stickyDragForce { get; private set; } = 100;
    [HideInInspector] public float bounceAmount = 10;
    [HideInInspector] public float windForce;
    [HideInInspector] public float windTriggerHeight;
    [HideInInspector] public GameObject windVisualPrefab;

    [HideInInspector] public BoxCollider windTrigger;



    public void SetColor(Material mat)
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = mat;
    }

    public void UpdateWindTiggerSize()
    {
        if (windTrigger == null) return;

        windTrigger.size = new Vector3(windTrigger.size.x, windTriggerHeight, windTrigger.size.z);
        windTrigger.center = new Vector3(windTrigger.center.x, windTriggerHeight/2, windTrigger.center.z);

    }
}


