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
        Debug.Log("Entered HuntState");
        brain = me.aiBrain;
        shootDistance = brain.shootDistance;

        me.onTheHunt = true;
    }

    public override void OnStateUpdate(StateManager me)
    {
        targetPos = brain.enemyLastKnowPos;
        hasLos = brain.hasLos;
        
        // if guy to to far away to shoot it should walk closer to target and try again if guy sees target
        var toTarget = (me.transform.position - targetPos).magnitude;

        if (toTarget > shootDistance)
        {
            me.walkerAgent.SetDestination(targetPos);
            return;
        }
        else
        {
            me.agent.ResetPath();
        }
        
        
            
            
        var angle = brain.AngleToTarget(targetPos);
        if (angle <= 0.99f ) 
            brain.RotateObject(targetPos);
        else
            brain._weapon.Shoot();
    }

    public override void OnStateExit(StateManager me)
    {
        Debug.Log("Exited HuntState");
        me.onTheHunt = false;
    }
}
