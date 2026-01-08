using Common.AI;
using Statemachine.Common;
using UnityEngine;

public class HuntState : StateMachineFactory
{

    private AiBrain brain;
    private float shootDistance;
    private Vector3 targetPos;
    private bool hasLos;
    
    public override void OnStateEnter(StateManager me)
    {
        brain = me.aiBrain;
        shootDistance = brain.shootDistance;

        me.onTheHunt = true;
    }

    public override void OnStateUpdate(StateManager me)
    {
        targetPos = brain.enemyLastKnowPos.transform.position;
        hasLos = brain.hasLos;
        
        // if guy to far away to shoot it should walk closer to target and try again if guy sees target
        var toTarget = (me.transform.position - targetPos).magnitude;

        if (!hasLos)
        {
            me.SwitchToLastKnownPosition();
        }
        
        if (toTarget > shootDistance)
        {
            me.walkerAgent.SetDestination(targetPos);
            return;
        }
        else
        {
            me.agent.ResetPath();
        }
    }

    public override void OnStateExit(StateManager me)
    {
        me.lastKnownPosition = targetPos;
        me.onTheHunt = false;
    }
}
