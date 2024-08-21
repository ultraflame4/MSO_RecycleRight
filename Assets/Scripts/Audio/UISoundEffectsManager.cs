using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundEffectsManager : MonoBehaviour
{
    [SerializeField] AudioClip click, back;

    /// <summary>
    /// To be called when a button is pressed to play sound effect
    /// </summary>
    public void Click()
    {
        SoundManager.Instance?.PlayOneShot(click);
    }

    /// <summary>
    /// To be called when a back button is pressed to play sound effect
    /// </summary>
    public void Back()
    {
        SoundManager.Instance?.PlayOneShot(back);
    }
}
