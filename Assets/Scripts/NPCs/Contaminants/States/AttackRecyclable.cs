using System.Collections;
using Interfaces;
using Level;
using NPC;
using NPC.Contaminant;
using NPC.Recyclable;
using Patterns.FSM;
using Player;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class AttackRecyclable : AttackTarget
    {
        public AttackRecyclable(ContaminantNPC npc) : base(npc)
        {
        }
        public RecyclableNPC nearestRecyclable;
        public override IDamagable target => nearestRecyclable;

        protected override void OnAttackTarget()
        {
            Debug.Log($"Attacking Recyclable {nearestRecyclable}");
            nearestRecyclable?.Contaminate(npc.attackDamage);
        }
    }
}