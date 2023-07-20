using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundTrackManager : MonoBehaviour
{
    private AudioSource _source;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        _source = GetComponent<AudioSource>();
    }

    public void ChangeVolume(float newVolume)
    {
        newVolume = Mathf.Clamp01(newVolume);

        _source.volume = newVolume;
    }
}
