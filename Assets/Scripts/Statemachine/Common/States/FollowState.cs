using UnityEngine;

namespace Statemachine.Common.States
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

            if(!me.aiBrain.seesEnemy)
                me.RotateOffsetFromLeader();
            else
            {
                Debug.Log("Sees Enemy: and Rotating");
                var ds = me.aiBrain.enemyLastKnowPos;
                me.aiBrain.SetVectorRotateTarget(ds);
            }
        }
        
        public override void OnStateExit(StateManager me)
        {
            me.walkerAgent.rotateGuy = !me.walkerAgent.rotateGuy;
        }
    }
}
