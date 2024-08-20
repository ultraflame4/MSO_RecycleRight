using System;
using System.Linq;
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
            if (_instance != this)
            {
                gameObject.SetActive(false);
            }
        }

        // get sources for sound effects
        sfxSources = GetComponents<AudioSource>();
        // play background music
        PlayBackgroundMusic(0);
    }

    /// <summary>
    /// Play a continuous sound effect
    /// </summary>
    /// <param name="clip">Audio clip to play</param>
    public void Play(AudioClip clip, bool loop = false)
    {
        if (sfxSources == null) return;

        AudioSource availableSource = null;

        if (sfxSources.Select(x => x.clip).Contains(clip)) return;

        // search for an available audio source to play sound
        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying) continue;
            availableSource = source;
            break;
        }

        // if there are no audio sources found, add a new one
        if (availableSource == null)
        {
            availableSource = gameObject.AddComponent<AudioSource>();
            sfxSources = sfxSources.Concat(new AudioSource[]{availableSource}).ToArray();
        }

        availableSource.clip = clip;
        availableSource.loop = loop;
        availableSource.Play();
    }

    /// <summary>
    /// Play a one-shot sound effect
    /// </summary>
    /// <param name="clip">Audio clip to play</param>
    public void PlayOneShot(AudioClip clip)
    {
        if (sfxSources == null) return;

        AudioSource availableSource = null;

        // search for an available audio source to play sound
        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying) continue;
            availableSource = source;
            break;
        }

        // if there are no audio sources found, add a new one
        if (availableSource == null)
        {
            availableSource = gameObject.AddComponent<AudioSource>();
            sfxSources = sfxSources.Concat(new AudioSource[]{availableSource}).ToArray();
        }
        
        // play audio
        availableSource.loop = false;
        availableSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Stop playing clip
    /// </summary>
    /// <param name="clip">Audio clip to stop playing</param>
    public void Stop(AudioClip clip)
    {
        // search for audio source with clip
        foreach (AudioSource source in sfxSources)
        {
            if (source.clip != clip) continue;
            source.clip = null;
            return;
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
