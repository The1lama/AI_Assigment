using UnityEngine;
using UnityEngine.AI;

namespace Statemachine.Friendly.States
{
    public class SearchState : StateMachineFactory
    {

        private float _range = 10.0f;
        
        public override void OnStateEnter(FriendlyStateManager me)
        {
            Vector3 point;
            if (RandomPoint(me.transform.position, _range, out point, me))
            {
                me.agent.SetDestination(point);
            }
        }

        public override void OnStateUpdate(FriendlyStateManager me)
        {
            if (me.agent.remainingDistance <= me.stopingDistance + 1f)
            {
                Vector3 point;
                if (RandomPoint(me.transform.position, _range, out point, me))
                {
                    me.agent.SetDestination(point);
                }
            }
        }

        public override void OnStateExit(FriendlyStateManager me)
        {
        }
        
        private bool RandomPoint(Vector3 center, float range, out Vector3 result, FriendlyStateManager me)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    if(me.view.GetDotProduct(hit.position - me.transform.position))
                    {
                        result = hit.position;
                        return true;
                    }
                }
            }
            result = Vector3.zero;
            return false;
        } 
    }
    
    
    
}
