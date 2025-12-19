using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Common.AI;

namespace Common.Lab3_Steering_Swarm.Scripts.AI
{
    public class SteeringAgent : MonoBehaviour
    {
        #region Variables

        private float maxSpeed;
        [Header("Movement")] 
        public float maxForce = 10f;

        [Header("Separation")] 
        public float separationRadius = 1.5f;
        public float separationStrength = 5f;
        
        [Header("Cohesion")]
        public float cohesionRadius = 5f;
        public float cohesionStrength = 0.17f;
        
        [Header("Alignment")]
        public float alignmentRadius = 3f;
        public float alignmentStrength = 2f;

        
        [Header("Weights")]
        public float separationWeight = 1f;
        public float cohesionWeight = 0.4f;
        public float alignmentWeight = 1f;
        public float avoidanceWeight = 1f;
        
        [Header("Debug")]
        public bool drawDebug = true;
        private Vector3 _velocity = Vector3.zero;
        private List<GameObject> allAgents;
        

        #endregion


        private void Awake()
        {
            allAgents = GameManager.Instance.allEntities;
        }

        private void Start()
        {
            maxSpeed = GetComponent<AiBrain>().speed;
        }
        
        public void OnUpdateSeparation()
        {
            Vector3 steering = Vector3.zero;
            
            
            if (allAgents.Count > 1)
            {
                steering += Separation(separationRadius, separationStrength) *  separationWeight;
                //steering += Cohesion(cohesionRadius, cohesionStrength) * cohesionWeight;
                //steering += Alignment(alignmentRadius, alignmentStrength) *  alignmentWeight;
                
            }

            if (steering == Vector3.zero) return;
            
            
            // Limit Steering (Truncate)
            steering = Vector3.ClampMagnitude(steering, maxForce);
            
            // Apply Steering to Velocity
            // Acceleration = Force / Mass. (We assume Mass = 1)
            // Velocity Change = Acceleration * Time.
            _velocity += steering * Time.deltaTime;
            _velocity = Vector3.ClampMagnitude(_velocity, maxForce);
            _velocity.y = 0;
            
            // Move Agent
            transform.position += _velocity * Time.deltaTime;
        }
        

        /// <summary>
        /// Advances to target without slowing down
        /// </summary>
        /// <param name="targetPos">To Target Position</param>
        /// <returns>Direction Force</returns>
        private Vector3 Seek(Vector3 targetPos)
        {
            var directionForce = targetPos - transform.position;
            if(directionForce.sqrMagnitude < 0.01f) return Vector3.zero;
            return (directionForce.normalized * maxSpeed) - _velocity;
        }

        /// <summary>
        /// Advances to target with braking in mind, so it slows down before reaching target
        /// </summary>
        /// <param name="targetPos">To Target Position</param>
        /// <param name="slowRadius">The radius before starting to slow down</param>
        /// <returns>Direction force</returns>
        private Vector3 Arrive(Vector3 targetPos, float slowRadius)
        {
            var distanceToTarget = targetPos - transform.position;
            var fDistance = distanceToTarget.magnitude;
            if(fDistance < 0.01f) return Vector3.zero;

            var desSpeed = maxSpeed;
            
            if (fDistance < slowRadius)
            {
                desSpeed = maxSpeed * (fDistance / slowRadius);
            }
            
            return distanceToTarget.normalized * desSpeed - _velocity;
        }

        /// <summary>
        /// Check neighbours and avoids them
        /// </summary>
        /// <param name="radius">The radius to keep the distance</param>
        /// <param name="strength">How hard the veering is</param>
        /// <returns>Direction Force</returns>
        private Vector3 Separation(float radius, float strength)    // Avoid distance from group
        {
            Vector3 seprarationForce = Vector3.zero;
            int neighbourCount = 0;
            

            foreach (var other in allAgents)
            {
                if(other == this.gameObject) continue;
                
                var toMe = transform.position - other.transform.position;
                var otherMagnitude = toMe.magnitude;


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

        private Vector3 Cohesion(float radius, float strength)       // tightness of the group 
        {
            Vector3 force = Vector3.zero;
            Vector3 avgPosition =  Vector3.zero;
            int neighbourCount = 0;

            foreach (var other in allAgents)
            {
                if(other == this) continue;
                
                var toMe = transform.position - other.transform.position;
                var otherMagnitude = toMe.magnitude;


                if (otherMagnitude > 0f && otherMagnitude < radius) // within range
                {
                    avgPosition += other.transform.position;
                    force += toMe.normalized / otherMagnitude;
                    neighbourCount++;
                }
            }

            if (neighbourCount > 0)
            {
                avgPosition /= neighbourCount;
            
                force = Seek(avgPosition) * maxSpeed;
                force = force - _velocity;
                force *= strength;
            }
            return force;
        }
        
        private Vector3 Alignment(float radius, float strength)     // the avg forward position
        {
            Vector3 avgDirection = Vector3.zero;
            int neighbourCount = 0;

            foreach (var other in allAgents)
            {
                if(other == this) continue;
                
                var toMe = transform.position - other.transform.position;
                var otherMagnitude = toMe.magnitude;

                if (otherMagnitude > 0f && otherMagnitude < radius) // within range
                {
                    avgDirection += other.transform.forward;    // stores all the forward positions from all the nodes within range
                    neighbourCount++;
                }
            }

            if (neighbourCount <= 0) return avgDirection;
            
            avgDirection /= neighbourCount;                     // takes the avg direction of all the close neighbours
            
            //avgDirection = Seek(avgDirection) * maxSpeed;
            avgDirection = avgDirection - _velocity;
            avgDirection *= strength;
            return avgDirection * maxSpeed;
        }

        private void OnDrawGizmos()
        {
            if (!drawDebug) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + _velocity);

        }
    }
}
