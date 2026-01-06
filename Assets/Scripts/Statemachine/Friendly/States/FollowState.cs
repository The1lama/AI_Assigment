using UnityEngine;

namespace Statemachine.Friendly.States
{
    public class FollowState : StateMachineFactory
    {

        
        public override void OnStateEnter(StateManager me)
        {
            me.walkerAgent.rotateGuy = !me.walkerAgent.rotateGuy;
        }

        public override void OnStateUpdate(StateManager me)
        {

            if (!me.agent.pathPending)
            {
                me.walkerAgent.SetDestination(me.leader.position);
            }

            me.RotateOffsetFromLeader();
        }
        
        public override void OnStateExit(StateManager me)
        {
            me.walkerAgent.rotateGuy = !me.walkerAgent.rotateGuy;
        }

        
    }
}
