using UnityEngine;

namespace Statemachine.Enemy.States
{
    public class FollowState : EnemyStateMachineFactory
    {
        public override void OnStateEnter(EnemyStateManager me)
        {
        }

        public override void OnStateUpdate(EnemyStateManager me)
        {

            if (!me.agent.pathPending)
            {
               me.SetAgentDestination(me.leader.position);
                
            }

            me.transform.position = Vector3.MoveTowards(me.transform.position, me.agent.nextPosition, 5 * Time.deltaTime);
                
            me.RotateOffsetFromLeader();
            
            me.steeringAgent.OnUpdateSeparation();
        }
        
        public override void OnStateExit(EnemyStateManager me)
        {
        }
    }
}
