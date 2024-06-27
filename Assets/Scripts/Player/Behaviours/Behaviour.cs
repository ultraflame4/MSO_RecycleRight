using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour : MonoBehaviour
{
    protected PlayerController character;

    void Start()
    {
        // get reference to player controller
        character = GetComponentInParent<PlayerController>();
        // check if character is null
        if (character != null) return;
        Debug.LogError("Componenet of type 'PlayerController' could not be found. (Behaviour.cs)");
    }

    // methods to be overrided depending on the character
    public virtual void TriggerAttack() {}
    public virtual void TriggerSkill() {}
}
