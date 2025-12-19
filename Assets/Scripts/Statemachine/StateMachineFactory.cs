using UnityEngine;


namespace Statemachine
{
    public abstract class StateMachineFactory
    {

        public abstract void OnStateEnter(StateManager me);
        
        public abstract void OnStateUpdate(StateManager me);
        
        public abstract void OnStateExit(StateManager me);
    }
}
