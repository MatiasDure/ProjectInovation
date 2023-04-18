using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOcclussion : MonoBehaviour
{
    [SerializeField] private List<Transform> lights;
    [SerializeField] Transform mainCamera; 

    [SerializeField] private float marginMultiplier = 1;


    private void FixedUpdate()
    {
        float b = 0.016923f * mainCamera.position.z + 3.1809f;
        b *= marginMultiplier;

        foreach (var light in lights)
        {
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(light.position);
            if (ViewportPosition.x < 0 - b || ViewportPosition.y < 0 - b || ViewportPosition.x > 1 + b || ViewportPosition.y > 1 + b)
            {
                light.gameObject.SetActive(false);
            }
            else light.gameObject.SetActive(true);
        }
    }

}
