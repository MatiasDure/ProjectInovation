using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOcclussion : MonoBehaviour
{
    [SerializeField] private List<Transform> lights;
    [SerializeField] private float margin;


    private void FixedUpdate()
    {
        foreach (var light in lights)
        {
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(light.position);
            if (ViewportPosition.x < 0 - margin || ViewportPosition.y < 0 - margin || ViewportPosition.x > 1 + margin || ViewportPosition.y > 1 + margin)
            {
                light.gameObject.SetActive(false);
            }
            else light.gameObject.SetActive(true);
        }
    }

}
