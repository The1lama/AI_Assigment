using UnityEngine;

namespace Statemachine.Enemy.States
{
    public class FollowState : EnemyStateMachineFactory
    {
        public override void OnStateEnter(EnemyStateManager me)
        {
            me.walkerAgent.rotateGuy = !me.walkerAgent.rotateGuy;
        }

        public override void OnStateUpdate(EnemyStateManager me)
        {

            if (!me.agent.pathPending)
            {
               me.walkerAgent.SetDestination(me.leader.position);
            }

            me.RotateOffsetFromLeader();
            
        }
        
        public override void OnStateExit(EnemyStateManager me)
        {
            me.walkerAgent.rotateGuy = !me.walkerAgent.rotateGuy;
        }
    }
}
