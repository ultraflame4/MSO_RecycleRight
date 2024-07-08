using System;
using System.Collections;
using UnityEngine;
using Entity.Data;

namespace Player.Behaviours
{
    public class Behaviour : MonoBehaviour
    {
        protected PlayerController character;
        protected PlayerCharacter data;

        // public events to be called when skill is triggered
        public event Action<PlayerCharacter> SkillTriggered;

        // handle skill cooldown
        Coroutine cooldown;
        protected bool canTriggerSkill = false;

        /// <summary>
        /// Property that returns if skill cooldown is over. Setting it to true would automatically delay the assignment of this property by the skill cooldown. 
        /// </summary>
        public bool CanTriggerSkill
        {
            get
            {
                return canTriggerSkill;
            }
            set
            {
                // if value is false, just set value
                if (!value)
                {
                    canTriggerSkill = value;
                    return;
                }

                // if value is true, check if coroutine is running
                if (cooldown != null) StopCoroutine(cooldown);
                // start a coroutine to count duration of skill cooldown
                cooldown = StartCoroutine(CountDuration(data.skillCooldown, () => 
                    {
                        canTriggerSkill = true;
                        cooldown = null;
                    }
                ));
            }
        }

        void Awake()
        {
            // get reference to player controller
            character = GetComponentInParent<PlayerController>();
            // get reference to character data script
            data = GetComponent<PlayerCharacter>();
            // check if character or is null
            if (character == null)
            {
                Debug.LogError("Component of type 'PlayerController' could not be found. (Behaviour.cs)");
                return;
            }
            // start skill cooldown
            CanTriggerSkill = true;
        }

        // methods to be overrided depending on the character
        public virtual void TriggerAttack() {}
        public virtual void TriggerSkill() 
        {
            // invoke event whenever skill is triggered
            SkillTriggered?.Invoke(data);
        }

        /// <summary>
        /// Coroutine to wait a certain amount of time before calling a method to perform an action
        /// </summary>
        /// <param name="duration">Duration to wait</param>
        /// <param name="callback">Method to call after the set duration</param>
        /// <returns></returns>
        protected IEnumerator CountDuration(float duration, Action callback = null)
        {
            yield return new WaitForSeconds(duration);
            callback?.Invoke();
        }
    }
}

