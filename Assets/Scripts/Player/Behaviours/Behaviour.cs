using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Data;

namespace Player.Behaviours
{
    public class Behaviour : MonoBehaviour
    {
        protected PlayerController character;
        protected PlayerCharacter data;

        // handle skill cooldown
        Coroutine cooldown;
        bool canTriggerSkill = false;

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
                // start a coroutine to count skill cooldown
                cooldown = StartCoroutine(WaitForCooldown(data.skillCooldown));
            }
        }

        void Start()
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
        public virtual void TriggerSkill() {}

        // coroutine to count the duration of the cooldown
        IEnumerator WaitForCooldown(float duration)
        {
            yield return new WaitForSeconds(duration);
            canTriggerSkill = true;
            cooldown = null;
        }
    }
}

