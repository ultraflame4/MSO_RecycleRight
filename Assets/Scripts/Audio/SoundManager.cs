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
        // set singleton instance
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple sound manager detected! This is not allowed!");

            // check if instance is the singleton instance
        }

        // get sources for sound effects
        sfxSources = GetComponents<AudioSource>();
        // play background music
        PlayBackgroundMusic(0);
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
        // check if there are any clips to play
        if (clips == null || clips.Length <= 0 || index < 0 || index >= clips.Length)
        {
            Debug.LogWarning($"Background music clip of index {index} could not be found! (SoundManager.cs)");
            return;
        }
        // check if current clip is already the new clip, if so, do not play
        if (bgSource.clip == clips[index] && bgSource.isPlaying) return;
        // set clip and play background music
        bgSource.clip = clips[index];
        bgSource.Play();
    }
}
