using System;
using System.Collections;
using UnityEngine;
using Entity.Data;
using Level.Bins;

namespace Player.Behaviours
{
    public class MaidBehaviour : BaseMeleeAttack
    {
        [Header("Skill")]
        [SerializeField] float skillRange = 5f;
        [SerializeField] float skillPullForce = 25f;
        [SerializeField] float skillDuration = 5f;
        [SerializeField] RecyclableType pullableType = RecyclableType.PAPER;
        [SerializeField] LayerMask pullableMask;

        public override void TriggerSkill()
        {
            base.TriggerSkill();
        }

        public new void OnDrawGizmosSelected() 
        {
            // draw gizmos of base state
            base.OnDrawGizmosSelected();
            // draw skill range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(character.transform.position, skillRange);
        }
    }
}
