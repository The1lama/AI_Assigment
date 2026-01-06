namespace Statemachine.Friendly
{
    public abstract class StateMachineFactory
    {

        public abstract void OnStateEnter(FriendlyStateManager me);
        
        public abstract void OnStateUpdate(FriendlyStateManager me);
        
        public abstract void OnStateExit(FriendlyStateManager me);
    }
}
