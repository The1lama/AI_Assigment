using System.Collections.Generic;
using UnityEngine;

namespace Statemachine.States
{
    public class FollowState : StateMachineFactory
    {

        
        public override void OnStateEnter(StateManager me)
        {
        }

        public override void OnStateUpdate(StateManager me)
        {

            if (!me.agent.pathPending)
            {
               me.SetAgentDestination(me.leader.position);
                
            }

            me.transform.position = Vector3.MoveTowards(me.transform.position, me.agent.nextPosition, 5 * Time.deltaTime);
                
            me.RotateOffsetFromLeader();
            
            me.steeringAgent.OnUpdateSeparation();
            
            
        }

        
        
        
        
        public override void OnStateExit(StateManager me)
        {
        }

        
    }
}
