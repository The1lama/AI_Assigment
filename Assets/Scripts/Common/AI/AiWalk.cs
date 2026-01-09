using System.Collections.Generic;
using Factory;
using UnityEngine;
using UnityEngine.AI;

namespace Common.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AiWalk : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private AiBrain _aiBrain;
        private float maxSpeed;
        [HideInInspector] public float stopingDistance;

        private Quaternion targetRotation;
        
        public bool rotateGuy = true;
        
        #region Seperation

            [Header("Movement")] 
            public float maxForce = 10f;

            [Header("Separation")] 
            public float separationRadius = 1.5f;
            public float separationStrength = 5f;
            
            [Header("Weights")]
            public float separationWeight = 1f;

        #endregion

        #region Debug

            [Header("Debug")]
            public bool drawDebug = true;
            private Vector3 _velocity = Vector3.zero;
            private List<GameObject> allAgents;

        #endregion

        private void Awake()
        {
            allAgents = GameManager.Instance.allEntities;
            _aiBrain = GetComponent<AiBrain>();
            maxSpeed = GetComponent<CharacterFactory>().speed;
            InitializeAgent();
            
            if (_agent == null)
            {
                Debug.LogError("No NavMeshAgent component found");
                return;
            }
        }

        private void InitializeAgent()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = maxSpeed;
            
            _agent.stoppingDistance = stopingDistance;
            _agent.updateRotation = false;
            _agent.updatePosition = false;
            _agent.autoRepath = true;
        }

        private void Update()
        {
            if (_agent == null) return;
            
            if(!_aiBrain.seperationOverride && (_aiBrain.leader != this.gameObject))
                OnUpdateSeparation();
            
            if(_agent.remainingDistance >= 0f)
                transform.position = Vector3.MoveTowards(transform.position, _agent.nextPosition, maxSpeed * Time.deltaTime);
            

            if (rotateGuy)
                UpdateRotation();
            
            
            _agent.nextPosition = transform.position;
        }

        private void UpdateRotation()
        {

            if (_aiBrain.seesEnemy)
            {
                targetRotation = _aiBrain._rotateGoal;
            }
            else
            {
                var steeringVelocity = GetSteering();
                if (steeringVelocity.sqrMagnitude < 0.0001f) return;
                
                targetRotation = Quaternion.LookRotation(steeringVelocity, Vector3.up);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, maxSpeed*Time.deltaTime);

        }
        

        public bool SetDestination(Vector3 destination, bool ignore = false)
        {
            if (_agent == null)
            {
                return false;
            }
            if (!_agent.pathPending || ignore)
            {
                return _agent.SetDestination(destination);
            }
            return false;
        }
        
        private void OnUpdateSeparation()
        {
            Vector3 steering = Vector3.zero;
            
            steering += Separation(separationRadius, separationStrength) *  separationWeight;

            if (steering == Vector3.zero) return;
            
            // Limit Steering speed
            steering = Vector3.ClampMagnitude(steering, maxForce);
            
            // Apply Steering to Velocity
            // Acceleration = Force / Mass. (We assume Mass = 1)
            // Velocity Change = Acceleration * Time.
            _velocity += steering * Time.deltaTime;
            _velocity = Vector3.ClampMagnitude(_velocity, maxForce);
            _velocity.y = 0f;
            
            // Move Agent
            transform.position += _velocity * Time.deltaTime;
            ConstrainToNavMesh();
            _agent.nextPosition = transform.position;
        }
        
        private Vector3 Separation(float radius, float strength)    // Avoid distance from group
        {
            Vector3 seprarationForce = Vector3.zero;
            int neighbourCount = 0;

            foreach (var other in allAgents)
            {
                if(other == this.gameObject) continue;
                
                var toMe = transform.position - other.transform.position;
                var otherMagnitude = toMe.magnitude;


                // if distance to other guy is more than 0 and less than seperation radius 
                // add to calculator
                if (otherMagnitude > 0f && otherMagnitude < radius)
                {
                    seprarationForce += toMe.normalized / otherMagnitude;
                    neighbourCount++;
                }
            }

            if (neighbourCount > 0)
            {
                seprarationForce /= neighbourCount;
                
                seprarationForce = seprarationForce.normalized * maxSpeed;
                seprarationForce = seprarationForce - _velocity;
                seprarationForce *= strength;
            }
            return seprarationForce;
        }

        public void UpdateStopingAgent(float stopingDistance)
        {
            _agent.stoppingDistance = stopingDistance;
        }

        /// <summary>
        /// Get the sering direction
        /// </summary>
        /// <returns>Normalized Vector3</returns>
        private Vector3 GetSteering()
        {
            if(!_agent.hasPath || _agent.path.corners.Length < 2) return Vector3.zero;
            
            var target = _agent.path.corners[1];
            var desirecVelocity = (target - transform.position).normalized;
            
            return desirecVelocity;
        }

        private void ConstrainToNavMesh()
        {
            if(NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                transform.position = hit.position;
        }
        
        
        private void OnDrawGizmos()
        {
            if (!drawDebug) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + _velocity);

        }
    }
}
