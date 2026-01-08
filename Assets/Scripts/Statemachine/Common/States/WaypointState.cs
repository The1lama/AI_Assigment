using System.Collections.Generic;
using System.Linq;
using Statemachine.Enemy;
using UnityEngine;

namespace Statemachine.Common.States
{
    public class WaypointState : StateMachineFactory
    {
        private Transform[] waypointsList;
        private int waypointIndex = 0;

        private bool GetWaypoints(StateManager me)
        {
            var enemy = me as EnemyStateManager;
            if (enemy == null || enemy.waypoints.Length <= 0) return false;
            
            waypointsList = enemy.waypoints;
            return true;
        }
        
        public override void OnStateEnter(StateManager me)
        {
            if (!GetWaypoints(me)) 
            {
                Debug.LogWarning("Waypoints not found");
                me.SwitchState(me.lastState);
                return;
            
            }
            waypointIndex = Random.Range(0, waypointsList.Length);
            NextWaypoint();
        }

        public override void OnStateUpdate(StateManager me)
        {
            if (me.agent.remainingDistance <= me.stopingDistance)
                me.walkerAgent.SetDestination(waypointsList[NextWaypoint()].transform.position);
        }

        private int NextWaypoint()
        {
            return (waypointIndex += 1) % waypointsList.Length;
        }
        

        public override void OnStateExit(StateManager me)
        {
        }
    }
}
