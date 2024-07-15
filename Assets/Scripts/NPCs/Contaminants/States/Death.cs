using Level;
using NPC;
using NPC.Contaminant;
using NPC.Recyclable;
using Patterns.FSM;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class Death : BaseRecyclableState
    {
        public Death(ContaminantNPC npc) : base(npc, npc)
        {

        }

        Vector3 dest;
        public override void Enter()
        {
            base.Enter();
            navigation.ClearDestination();
            if (GeneralBinSingleton.instance == null)
            {
                GameObject.Destroy(transform.gameObject);
                return;
            }
            dest = GeneralBinSingleton.instance.worldPosition;
            navigation.enabled = false;
        }

        public override void Exit()
        {
            base.Exit();
        }


        public override void LogicUpdate()
        {
            base.LogicUpdate();
            var v = (dest - transform.position);
            var dist = v.magnitude;
            var dir = v.normalized;
            transform.position += dir * Mathf.Clamp01(1 - Mathf.Clamp01(dist) + 0.1f);
            if (dist < 0.1f)
            {
                GameObject.Destroy(transform.gameObject);
            }
        }
    }
}