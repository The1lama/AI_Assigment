using System.Collections.Generic;
using System.Linq;
using Statemachine.Enemy;
using UnityEngine;

namespace Statemachine.Common.States
{
    public class WaypointState : StateMachineFactory
    {
        public List<GameObject> waypointsList = new List<GameObject>();
        private int waypointIndex = 0;

        private bool GetWaypoints()
        {
            var wayState = GameObject.FindGameObjectsWithTag("Waypoint");
            if(wayState == null) return false;
            
            waypointsList = wayState.ToList();
            return true;
        }
        
        public override void OnStateEnter(StateManager me)
        {
            if (waypointsList.Count <= 0)
            {
                var beel = GetWaypoints();
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

        public override void OnStateUpdate(StateManager me)
        {
            if (me.agent.remainingDistance <= me.stopingDistance + 1f)
            {
                var indexPoint = NextWaypoint();
                me.walkerAgent.SetDestination(waypointsList[indexPoint].transform.position);
            }
        }

        private int NextWaypoint()
        {
            waypointIndex = (waypointIndex + 1) % waypointsList.Count;
            return waypointIndex;
        }
        

        public override void OnStateExit(StateManager me)
        {
        }
    }
}
