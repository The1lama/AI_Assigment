using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Statemachine.Enemy.States
{
    public class WaypointState : EnemyStateMachineFactory
    {
        public List<Transform> waypointsList = new List<Transform>();
        private int waypointIndex = 0;

        private bool GetWaypoints()
        {
            var wayState = GameObject.Find("===WaypointParrent===");
            if(wayState == null) return false;
            Debug.Log(wayState);
            waypointsList = wayState.GetComponentsInChildren<Transform>().ToList();
            return true;
        }
        
        public override void OnStateEnter(EnemyStateManager me)
        {
            if (waypointsList.Count <= 0)
            {
                var beel = GetWaypoints();
                Debug.Log(beel);

                if (!beel)
                {
                    Debug.LogWarning("Waypoints not found");
                    me.SwitchState(me.lastState);
                    return;
                }
                
            }
            
            waypointIndex = Random.Range(0, waypointsList.Count);
            NextWaypoint();
        }

        public override void OnStateUpdate(EnemyStateManager me)
        {
            if (me.agent.remainingDistance <= me.separationDistance + 1f)
            {
                var indexPoint = NextWaypoint();
                me.agent.SetDestination(waypointsList[indexPoint].position);
            }
        }

        private int NextWaypoint()
        {
            waypointIndex = (waypointIndex + 1) % waypointsList.Count;
            return waypointIndex;
        }
        

        public override void OnStateExit(EnemyStateManager me)
        {
        }
    }
}
