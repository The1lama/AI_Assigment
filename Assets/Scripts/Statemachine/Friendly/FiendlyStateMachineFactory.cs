namespace Statemachine.Friendly
{
    public abstract class FiendlyStateMachineFactory
    {

        public abstract void OnStateEnter(FriendlyStateManager me);
        
        public abstract void OnStateUpdate(FriendlyStateManager me);
        
        public abstract void OnStateExit(FriendlyStateManager me);
    }
}
