using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        Jump,
        Stick,
        PlayerSelected,
        Bounce,
        Win,
        PlayerJoin
    }

    [SerializeField] Sounds[] _sounds;

    AudioSource _source;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this) Destroy(this.gameObject);

        _source = GetComponent<AudioSource>();
    }

    public void PlaySound(Sound soundToPlay)
    {
        Debug.Log(soundToPlay);
        AudioClip[] clips = FindAudioClipsBySound(soundToPlay);

        AudioClip clipToPlay = clips.Length > 1 ? GetRandomAudioClip(clips) : clips[0];

        _source.PlayOneShot(clipToPlay);
    }

    private AudioClip GetRandomAudioClip(AudioClip[] pClips) => pClips[Random.Range(0, pClips.Length)];

    private AudioClip[] FindAudioClipsBySound(Sound query)
    {
        foreach(var sound in _sounds)
        {
            if (sound.sound != query) continue;
                
            return sound.audioClips;
        }

        return null;
    }
}

[System.Serializable]
public struct Sounds
{
    public SoundManager.Sound sound;
    public AudioClip[] audioClips;
}
