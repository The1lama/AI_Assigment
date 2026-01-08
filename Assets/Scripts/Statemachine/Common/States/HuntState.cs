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
        
            
        var angle = brain.AngleToTarget(brain._weapon.transform.forward, targetPos.normalized);
        if (angle <= 0.98f)
        {
            brain.SetVectorRotateTarget(targetPos);
        }
        else
        {
            brain._weapon.Shoot();
            Debug.Log("Shoooot");
        }
    }

    public override void OnStateExit(StateManager me)
    {
        Debug.Log("Exited HuntState");
        Debug.Log($"<Color=red>TryAndSee Sucsessfull: {targetPos}</Color>");
        me.lastKnownPosition = targetPos;
        me.onTheHunt = false;
    }
}
