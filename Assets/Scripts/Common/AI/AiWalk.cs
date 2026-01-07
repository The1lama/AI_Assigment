using System;
using System.Collections.Generic;
using Factory;
using Unity.Mathematics;
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
            _agent.radius = separationRadius;
        }

        private void Update()
        {
            if (_agent == null) return;
            
            OnUpdateSeparation();
            if(_agent.remainingDistance >= 0f)
                transform.position = Vector3.MoveTowards(transform.position, _agent.nextPosition, maxSpeed * Time.deltaTime);
            

            if (rotateGuy)
            {
                UpdateRotation();
            }
        }

        private void UpdateRotation()
        {
            var steeringVelocity = GetSteering();
            if (steeringVelocity.magnitude < 0.001f) return;
            var targetRot = Quaternion.LookRotation(steeringVelocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, maxSpeed * Time.deltaTime);
            
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_agent.transform.forward), maxSpeed * Time.deltaTime);
        }

        public void SetDestination(Vector3 destination)
        {
            if (!_agent.pathPending)
            {
                _agent.SetDestination(destination);
            }
        }
        
        private void OnUpdateSeparation()
        {
            Vector3 steering = Vector3.zero;


            //if (!(_agent.remainingDistance <= _agent.stoppingDistance))
              //  steering += GetPathStreeing();
            
            
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
