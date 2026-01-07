using System.Collections.Generic;
using Common;
using Common.AI;
using Statemachine.Common;
using Statemachine.Common.States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Statemachine.Enemy
{
    public class EnemyStateManager : StateManager
    {

        #region States
        
        public enum State
        {
            Waypoint,
            Follow,
            Hold,
            Search,
            Medi,
        }

        public override StateMachineFactory currentState { get; set; }

        public override StateMachineFactory[] stateList { get; set; } = new StateMachineFactory[]
        {
            new WaypointState(),
            new FollowState(),
            new HoldState(),
            new SearchState(),
            new MediState(),
        };
        public override StateMachineFactory lastState { get; set; }

        #endregion

        #region Squad

        public override float offsetAngle { get; set; }
        public override Transform leader { get; set; }
        public override bool isLeader { get; set; } = false;
        public override List<GameObject> _group { get; set; } = new List<GameObject>();
        public override float stopingDistance { get; set; }
        public override LayerMask teamLayerMask { get; set; }
        public override bool lastAlive { get; set; }

        #endregion

        #region Movements & Senses

        public override NavMeshAgent agent { get; set; }
        public override AiWalk walkerAgent { get; set; }
        public override SensingView view { get; set; }

        public Transform[] waypoints;
        
        
        #endregion

        #region Medic Class

        public override bool isMedi { get; set; }
        public override float helpRadius { get; set; }
        public override bool onHealingRoute { get; set; }
        public override List<GameObject> hurtComrades { get; set; }
        public override void CheckToSwitchLeader()
        {
            if (_group.Count > 1)   // 2 or more
            {
                Debug.Log("Switching to " + _group.Count + " leaders");
                
                // gets a new leader thats not a medic
                foreach (var comradeAlive in _group)
                {
                    if (comradeAlive == null || comradeAlive.GetComponent<AiBrain>().isMedi) continue;
                    if(comradeAlive != this.gameObject)
                        leader = comradeAlive.transform;
                    else
                    {
                        SwitchState(stateList[(int)State.Search]);
                        isLeader = true;
                    }
                    break;
                }
            }
            else
            {
                Debug.Log("LastAlive");
                lastAlive = true;
                SwitchState(stateList[(int)State.Search]);
            }
        }

        public override void IfMedic()
        {
            if(!onHealingRoute && FindHurtComrades().Count > 0)
                SwitchState(stateList[(int)State.Medi]);
        }

        #endregion
        
        #region KeyBinds
        
        private InputAction waypointAction;
        
        private void OnEnable()
        {
            waypointAction = new InputAction(
                name: "SearchAction",
                type: InputActionType.Button,
                binding: "<Keyboard>/l"
            );
            waypointAction.performed += WaypointActionOnperformed;
            waypointAction.Enable();
        }

        private void WaypointActionOnperformed(InputAction.CallbackContext obj)
        {
            SwitchState(stateList[(int)State.Waypoint]);
        }

        private void OnDisable()
        {
            if(waypointAction != null) {waypointAction.performed -= WaypointActionOnperformed; waypointAction.Disable();}
        }
            
        #endregion


        public override void Awake()
        {
            base.Awake();
            
            SwitchState(stateList[(int)State.Waypoint]);
        }
    }
}
