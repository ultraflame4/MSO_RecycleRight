using System.Linq;
using Level;
using Level.Bins;
using NPC;
using NPC.Contaminant;
using NPC.Recyclable;
using Patterns.FSM;
using Player;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class ChaseBin : DetectTarget
    {

        public RecyclingBin targetBin;
        public ChaseBin(ContaminantNPC npc) : base(npc)
        {
        }


        public override void Enter()
        {
            base.Enter();
            // Navigation component may be disabled!
            if (navigation != null && navigation.enabled)
            {
                navigation.ClearDestination();
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // todo, use layer mask to filter colliders
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, npc.sightRange);
            var nearest = colliders.Select(x => x.GetComponent<RecyclingBin>()).Where(x => x != null).OrderBy(x => (x.transform.position - npc.transform.position).sqrMagnitude).FirstOrDefault();
            if (nearest)
            {
                // Navigation component may be disabled!
                if (navigation != null && navigation.enabled)
                {
                    navigation.SetDestination(nearest.transform.position);
                }
                return;
            }
            npc.SwitchState(npc.state_Idle);
        }

        public override void Exit()
        {
            base.Exit();
            navigation.ClearDestination();
        }
    }
}