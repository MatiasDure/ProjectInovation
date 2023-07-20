using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColorsHolder : MonoBehaviour
{
    public static ColorsHolder Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this; 
    }

    [SerializeField] public Material white; 
    [SerializeField] public Material red;
    [SerializeField] public Material blue; 
    [SerializeField] public Material yellow; 
    [SerializeField] public Material green;
    [SerializeField] public GameObject windVisual;
    [SerializeField] private AudioSource windSource;

    [SerializeField] public PhysicMaterial ice;

    [SerializeField] public Transform healthInfoContainer; 

    public void PlayWindSound()
    {
        if (windSource.isPlaying) return;

        windSource.Play();
    }
}
