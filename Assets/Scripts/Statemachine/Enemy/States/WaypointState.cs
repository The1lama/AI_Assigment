using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Statemachine.Enemy.States
{
    public class WaypointState : EnemyStateMachineFactory
    {
        public List<GameObject> waypointsList = new List<GameObject>();
        private int waypointIndex = 0;

        private bool GetWaypoints()
        {
            var wayState = GameObject.FindGameObjectsWithTag("Waypoint");
            if(wayState == null) return false;
            Debug.Log(wayState);
            waypointsList = wayState.ToList();
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
            Debug.Log(waypointsList.Count);
            waypointIndex = Random.Range(0, waypointsList.Count);
            NextWaypoint();
        }

        public override void OnStateUpdate(EnemyStateManager me)
        {
            if (me.agent.remainingDistance <= me.stopingDistance + 1f)
            {
                var indexPoint = NextWaypoint();
                me.walkerAgent.SetDestination(waypointsList[indexPoint].transform.position);
                //me.agent.SetDestination(waypointsList[indexPoint].transform.position);
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
