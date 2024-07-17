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
    public class AttackPlayer : AttackTarget
    {
        public AttackPlayer(ContaminantNPC npc) : base(npc)
        {
        }
        public override IDamagable target {
            get {
                if (PlayerController.Instance == null) return null;
                if (Vector3.Distance(PlayerController.Instance.transform.position, npc.transform.position) > npc.attackRange) return null;
                return PlayerController.Instance.Data;
            }
        }
    }
}