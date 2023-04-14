using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AdaptWind : MonoBehaviour
{
    public void AdaptToObject(Transform windObject)
    {
        windObject.TryGetComponent<Surface>(out var parentSurface);
        Debug.Log(parentSurface.windTriggerHeight);
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        //1:5
        foreach(var particle in particleSystems)
        {
            var main = particle.main;
            
            //set lifespan
            float newStartLife = parentSurface.windTriggerHeight / 5f;
            main.startLifetime = newStartLife;

            //set speed
            main.startSpeed = newStartLife / 2f;

            //set size
            var shapeModule = particle.shape;
            shapeModule.scale = new Vector3(windObject.lossyScale.z * 2f, windObject.lossyScale.x * 2f, 0);
        }
    }
}
