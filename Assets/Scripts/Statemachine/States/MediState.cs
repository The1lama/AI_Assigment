using Factory;
using Statemachine;
using UnityEngine;



namespace Statemachine.States
{
    public class MediState : StateMachineFactory
    {
        
        public override void OnStateEnter(StateManager me)
        {
            me.onHealingRoute =  true;
        }

        public override void OnStateUpdate(StateManager me)
        {
            Debug.Log("Updating Medi State");
            foreach (var comradeObject in me.hurtComrades)
            {
                Debug.Log(comradeObject.name);
                if(comradeObject == null) continue;
                var comrade = comradeObject.GetComponent<CharacterFactory>();
                Debug.Log(comrade.name);
                while (comrade.needsHealth)
                {
                    var distanceToComrade = Vector3.Distance(me.transform.position, comradeObject.transform.position);
                    Debug.Log(distanceToComrade);
                    if (distanceToComrade < 3)
                    {
                        comrade.TakeHeal(10);
                    }
                    else
                    {
                        me.agent.destination = comradeObject.transform.position;
                    }
                }
                
            }
            Debug.Log("Switch State");
            me.SwitchState(me.lastState);
        }

        public override void OnStateExit(StateManager me)
        {
            me.onHealingRoute = false;
        }
    }
}
