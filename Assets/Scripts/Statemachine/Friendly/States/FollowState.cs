using UnityEngine;

namespace Statemachine.Friendly.States
{
    public class FollowState : StateMachineFactory
    {

        
        public override void OnStateEnter(FriendlyStateManager me)
        {
            me.walkerAgent.rotateGuy = !me.walkerAgent.rotateGuy;
        }

        public override void OnStateUpdate(FriendlyStateManager me)
        {

            if (!me.agent.pathPending)
            {
                me.walkerAgent.SetDestination(me.leader.position);
            }

            me.RotateOffsetFromLeader();
        }
        
        public override void OnStateExit(FriendlyStateManager me)
        {
            me.walkerAgent.rotateGuy = !me.walkerAgent.rotateGuy;
        }

        
    }
}
