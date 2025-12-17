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

        public bool GetDotProduct(Vector3 toTarget)
        {
            var dotProd = Vector3.Dot(transform.TransformDirection(Vector3.forward), (toTarget).normalized);
            var cosineThreshold = Mathf.Cos(fov * Mathf.Deg2Rad * 0.5f);
            return dotProd >= cosineThreshold;
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

            if (!GetDotProduct(toTarget)) return false;

            // Check LIne of sight from guard to target 
            if (!TryLineOfSight(target)) return false;
            
            lastKnownPosition = target.position;
            hasLineOfSight = true;
            toTargetDistance = toTarget.magnitude;
            return true;
        }
    }
}

