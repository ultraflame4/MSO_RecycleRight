using UnityEngine;
using Patterns.FSM;
using Bosses.Pilotras.Projectile;
using System.Collections;

namespace Bosses.Pilotras.FSM
{
    public class LaneAttackState : CooldownState<PilotrasController>
    {
        public LaneAttackState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.lane_attack_duration + character.data.attack_delay, character.behaviourData.lane_attack_cooldown)
        {
        }

        public override void Enter()
        {
            cooldown = character.behaviourData.lane_attack_cooldown;
            base.Enter();
            SpawnProjectile();
        }

        void SpawnProjectile()
        {
            Vector2 spawnPos = new Vector2(character.data.minBounds.x, Random.Range(character.data.minBounds.y, character.data.maxBounds.y));
            GameObject obj = character.SpawnProjectile(new Vector2(spawnPos.x, character.yPosTop), out ProjectileController projectile);
            if (obj == null || projectile == null) return;
            // get npc prefab and sprites
            GameObject prefab = character.data.currentPhaseNPCs[Random.Range(0, character.data.currentPhaseNPCs.Length)];
            SpriteRenderer prefabSprite = prefab.transform.GetChild(1).GetComponentInChildren<SpriteRenderer>();
            // set projectile properties
            projectile.spriteRenderer.sprite = prefabSprite.sprite;
            projectile.damage = character.data.lane_attack_damage;
            projectile.maxX = character.data.maxBounds.x;
            projectile.npcPrefab = prefab;
            // delay activating projectile
            projectile.enabled = false;
            character.StartCoroutine(DelayedProjectileActivation(projectile, spawnPos));
        }

        IEnumerator DelayedProjectileActivation(ProjectileController projectile, Vector2 spawnPos)
        {
            if (projectile != null) projectile.spriteRenderer.enabled = false;
            yield return new WaitForSeconds(character.data.attack_delay - character.behaviourData.drop_speed);
            // throw projectile into position
            if (projectile != null) projectile.spriteRenderer.enabled = true;
            character.StartCoroutine(character.Throw(character.behaviourData.drop_speed, projectile.gameObject, spawnPos));
            // wait for remaining time before launching projectile
            yield return new WaitForSeconds(character.behaviourData.drop_speed);
            // activate projectile after wait duration
            if (projectile != null) projectile.enabled = true;
        }
    }
}
