using UnityEngine;
using UnityEngine.AI;

namespace Common.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AiWalk : MonoBehaviour
    {
        private NavMeshAgent _agent;
        
        
        
        public void InitializeAgent(float speed)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;
        }
        
        public void SetDestination(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }
        
    }
}
