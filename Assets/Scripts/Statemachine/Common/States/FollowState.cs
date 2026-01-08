using UnityEngine;

namespace Statemachine.Common.States
{
    public class FollowState : StateMachineFactory
    {
        public override void OnStateEnter(StateManager me)
        {
            me.walkerAgent.rotateGuy = !me.walkerAgent.rotateGuy;
            me.aiBrain.rotateOverride =  !me.aiBrain.rotateOverride;
        }

        public override void OnStateUpdate(StateManager me)
        {

            if (!me.agent.pathPending)
            {
                me.walkerAgent.SetDestination(me.leader.position);
            }

            if(!me.aiBrain.seesEnemy ||me.aiBrain.enemyLastKnowPos == null)
                me.RotateOffsetFromLeader();
            else if(me.aiBrain.enemyLastKnowPos != null)
            {
                var ds = me.aiBrain.enemyLastKnowPos.transform.position;
                me.aiBrain.SetVectorRotateTarget(ds);
            }
        }
        
        public override void OnStateExit(StateManager me)
        {
            me.walkerAgent.rotateGuy = !me.walkerAgent.rotateGuy;
            me.aiBrain.rotateOverride =  !me.aiBrain.rotateOverride;
        }
    }
}
