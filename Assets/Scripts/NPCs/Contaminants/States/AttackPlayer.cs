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
        public override IDamagable target => PlayerController.Instance;
    }
}