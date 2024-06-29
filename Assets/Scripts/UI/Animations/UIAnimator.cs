using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class UIAnimator : MonoBehaviour
{
    [SerializeField] UIAnimation[] animations;

    // Start is called before the first frame update
    void Start()
    {
        // play first animation
        animations[0].Play();
    }

    public void Play(string name)
    {
        // if animations list do not contain animation with the name
        // log it and do not run
        if (!animations.Select(x => x.Name).Contains(name))
        {
            Debug.LogWarning("Animation of name '" + name + "' could not be found. (UIAnimator.cs)");
            return;
        }
        // stop other animations
        foreach (UIAnimation animation in animations.Where(x => x.Name != name))
        {
            animation.Stop();
        }
        // play animation with the name
        animations.Where(x => x.Name == name).ToArray()[0].Play();
    }
}
