using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Behaviour : MonoBehaviour
{
    protected PlayerController character;

    void Start()
    {
        // get reference to player controller
        character.GetComponent<PlayerController>();
    }

    // methods to be overrided depending on the character
    public virtual void TriggerAttack() {}
    public virtual void TriggerPassive() {}
    public virtual void TriggerSkill() {}
}
