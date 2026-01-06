namespace Statemachine.Enemy
{
    public abstract class EnemyStateMachineFactory
    {

        public abstract void OnStateEnter(EnemyStateManager me);
        
        public abstract void OnStateUpdate(EnemyStateManager me);
        
        public abstract void OnStateExit(EnemyStateManager me);
    }
}
