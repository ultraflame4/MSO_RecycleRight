using System;
using System.Collections;
using UnityEngine;
using Patterns.FSM;
using Entity.Data;
using Level;

namespace Player.FSM
{
    public class PlayerDeathState : State<PlayerController>
    {
        Coroutine coroutine;

        public PlayerDeathState(StateMachine<PlayerController> fsm, PlayerController character) : base(fsm, character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            // ensure only one death coroutine is running
            if (coroutine != null) return;
            // start coroutine to wait for death duration
            coroutine = character.StartCoroutine(WaitForDeath());
            // play death animation (hit animation)
            character.anim?.Play("On Hit");
        }

        IEnumerator WaitForDeath()
        {
            float duration = 0f;
            // fade character
            while (duration < character.Data.deathDuration)
            {
                Color newColor = character.Data.renderer.color;
                newColor.a -=  Time.deltaTime / character.Data.deathDuration;
                character.Data.renderer.color = newColor;
                // increment coroutine
                duration += Time.deltaTime;
                yield return duration;
            }
            HandleDeathDurationEnd();
        }

        int GetNextAvailableCharacter()
        {
            int index = Array.IndexOf(character.CharacterManager.character_instances, character.Data);
            int defaultReturnValue = -1;
            for (int i = 0; i < character.CharacterManager.character_instances.Length - 1; i++)
            {
                index++;
                if (index >= character.CharacterManager.character_instances.Length) index = 0;
                if (character.CharacterManager.character_instances[index].IsCleaning) defaultReturnValue = -2;
                if (!character.CharacterManager.character_instances[index].Switchable) continue;
                return index;
            }
            return defaultReturnValue;
        }

        void HandleDeathDurationEnd()
        {
            // when character dies, switch to next available character in party
            int index = GetNextAvailableCharacter();

            // if there is a character found, switch to that character
            if (index >= 0)
            {
                ResetAlpha();
                ExitState(index);
                return;
            }
            
            // if index is -2, it is not the end of the game, there are still available chracters
            // but they are currently busy, so wait until they can be swapped into
            if (index == -2)
            {
                foreach (PlayerCharacter chara in character.CharacterManager.character_instances)
                {
                    if (!chara.IsCleaning) continue;
                    chara.GetComponent<BinCleaning.BinCleaning>().lastCharacter = true;
                    character.PointerManager.gameObject.SetActive(false);
                    character.CharacterManager.CharacterChanged += CharacterAvailable;
                    return;
                }
            }

            // if index is -1, it is the end of the game
            LevelManager.Instance?.EndLevel(true);
        }

        // when character is available, this would be called, and would switch to new character
        void CharacterAvailable(PlayerCharacter prev, PlayerCharacter curr)
        {
            ResetAlpha();
            character.PointerManager.gameObject.SetActive(true);
            character.CharacterManager.CharacterChanged -= CharacterAvailable;
            ExitState(Array.IndexOf(character.CharacterManager.character_instances, curr));
        }

        void ResetAlpha()
        {
            // this is temp just for the alpha changing death animation
            Color newColor = character.Data.renderer.color;
            newColor.a = 1f;
            character.Data.renderer.color = newColor;
        }

        void ExitState(int characterIndex)
        {
            // reset coroutine
            if (coroutine != null) character.StopCoroutine(coroutine);
            coroutine = null;
            // reset animation to idle before switching characters
            character.anim?.Play("Idle");
            // allow switching characters, switch to new character, and exit death state
            character.CharacterManager.CanSwitchCharacters = true;
            character.CharacterManager.SwitchCharacter(characterIndex);
            fsm.SwitchState(character.DefaultState);
        }
    }
}
