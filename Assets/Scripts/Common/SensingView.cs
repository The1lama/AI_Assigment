using System;
using UnityEngine;

namespace Common
{
    public class SensingView : MonoBehaviour
    {
        [Header("Target Layer")]
        public LayerMask targetLayer { get; set; }

        [Header("View")]
        public float viewingDistance { get; set; }
        public float fov { get; set; }
        public LayerMask obstructionLayerMask { get; set; }
        
        [SerializeField] private bool isInRangeAndSeen;

        [Header("Debug"), SerializeField] private bool isDebug = true;

        /// <summary>
        /// Checks if object has line of sight to Target
        /// </summary>
        /// <param name="target">Target transform</param>
        /// <returns>True: If targets in line of sight; False: If targets not in line of sight</returns>
        private bool TryLineOfSight(Transform target)
        {
            if (!Physics.Linecast(transform.position, target.position, obstructionLayerMask))
            {
                Debug.DrawRay(transform.position, target.position - transform.position, Color.green);
                return true;
            }
            Debug.DrawRay(transform.position, target.position - transform.position, Color.red);
            return false;
        }

        /// <summary>
        /// Check the distance to the target 
        /// </summary>
        /// <param name="toTarget">To target distance magnitude</param>
        /// <returns>True: If targets within viewing Distance False: If targets outside viewing Distance</returns>
        private bool WithinDistanceToTarget(float toTarget)
        {
            return toTarget < viewingDistance + 0.5f;
        }

        public bool TrySeeTarget(Transform target, out Vector3 lastKnownPosition, out bool hasLineOfSight, out float toTargetDistance)
        {
            // Check flow
            // 1. Distance
            // 2. Cone Vision
            // 3. Line of sight 
            lastKnownPosition = default;
            hasLineOfSight = false;
            toTargetDistance = 99999f;
            
            var toTarget = target.position - transform.position;


            if (!WithinDistanceToTarget(toTarget.magnitude)) return false;      // Gets the distance from guard to target

            // Check the cone vision from guard to target if the targets in view
            var dotProd = Vector3.Dot(transform.TransformDirection(Vector3.forward), (toTarget).normalized);
            var cosineThreshold = Mathf.Cos(fov * Mathf.Deg2Rad * 0.5f);
            isInRangeAndSeen = dotProd >= cosineThreshold;
            if (!isInRangeAndSeen) return false;

            // Check LIne of sight from guard to target 
            if (!TryLineOfSight(target)) return false;
            
            lastKnownPosition = target.position;
            hasLineOfSight = true;
            toTargetDistance = toTarget.magnitude;
            return true;
        }
        
        private void OnDrawGizmos()
        {
            if (isDebug)
            {
                Gizmos.color = isInRangeAndSeen ? Color.green : Color.red;
                Gizmos.DrawWireSphere(transform.position, viewingDistance);
                
                
                Vector3 rightBoundary = Quaternion.Euler(0, fov * 0.5f, 0) * transform.TransformDirection(transform.forward);
                Vector3 leftBoundary = Quaternion.Euler(0, -fov * 0.5f, 0) * transform.TransformDirection(transform.forward);

                // gets shows wrong direction when facing -z dont care enough to fix right now.
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewingDistance);
                Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewingDistance);
                
            }
        }
    }
}

