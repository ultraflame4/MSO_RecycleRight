using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] AudioSource bgSource;
    [SerializeField] AudioClip[] clips;

    AudioSource[] sfxSources;

    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
                throw new NullReferenceException("There is no Game Manager in the scene!");
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple sound manager detected! This is not allowed!");
        }
    }

    void Start()
    {
        sfxSources = GetComponents<AudioSource>();
    }

    /// <summary>
    /// Play a one-shot sound effect
    /// </summary>
    /// <param name="clip">Audio clip to play</param>
    public void Play(AudioClip clip)
    {
        if (sfxSources == null) return;

        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying || source == sfxSources[^1]) continue;
            source.clip = clip;
            source.PlayOneShot(clip);
            break;
        }
    }

    /// <summary>
    /// Play a background music
    /// </summary>
    /// <param name="index">Index of audio clip in clips array</param>
    public void PlayBackgroundMusic(int index)
    {
        if (clips == null || clips.Length <= 0 || index < 0 || index >= clips.Length) return;
        bgSource.clip = clips[index];
        bgSource.Play();
    }
}
