using System.Collections.Generic;
using Common;
using Common.AI;
using Statemachine.Common;
using Statemachine.Common.State;
using Statemachine.Common.States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Statemachine.Friendly
{
    
    public class FriendlyStateManager : StateManager
    {

        #region States
        public enum State
        {
            Follow,
            Hold,
            Search,
            Medi,
            Hunt,
            LastKnowPos,
            
        }

        public State startState = new State();
        
        public override StateMachineFactory defaultState { 
            get => stateList[(int)startState]; 
            set => stateList[(int)startState] = value; 
        }
        public override StateMachineFactory currentState { get; set; }

        public override StateMachineFactory[] stateList { get; set; } = new StateMachineFactory[]
        {
            new FollowState(),
            new HoldState(),
            new SearchState(),
            new MediState(),
            new HuntState(),
            new LastKnownPosState(),
        };
        public override StateMachineFactory lastState { get; set; }
        public override Vector3 lastKnownPosition { get; set; }

        #endregion

        #region Squad

        public override float offsetAngle { get; set; }
        public override Transform leader { get; set; }
        public override bool isLeader { get; set; } = false;
        public override List<GameObject> _group { get; set; } = new List<GameObject>();
        public override float stopingDistance { get; set; }

        public override LayerMask teamLayerMask
        {
            get => gameObject.layer; 
            set => gameObject.layer = value;
        }
        public override bool lastAlive { get; set; } = false;

        #endregion

        #region Movements & Senses

        public override NavMeshAgent agent { get; set; }
        public override AiWalk walkerAgent { get; set; }
        public override SensingView view { get; set; }

        #endregion

        #region Medic Class

        public override bool isMedi { get; set; }

        public override float helpRadius { get; set; }
        public override bool onHealingRoute { get; set; } = false;
        public override List<GameObject> hurtComrades { get; set; } = new List<GameObject>();

        public override void IfMedic()
        {
            if(!onHealingRoute && FindHurtComrades().Count > 0)
                SwitchState(stateList[(int)State.Medi]);
        }

        #endregion
        
        #region KeyBinds
            private InputAction holdAction;
            private InputAction followAction;
            private InputAction mediAction { get; set; }
            private InputAction searchAction;

            private void OnEnable()
            {
                holdAction = new InputAction(
                    name: "HoldAction",
                    type: InputActionType.Button,
                    binding: "<Keyboard>/h"
                    );
                holdAction.performed += HoldActionOnperformed;
                holdAction.Enable();

                followAction = new InputAction(
                    name: "FollowAction",
                    type: InputActionType.Button,
                    binding: "<Keyboard>/f"
                );
                followAction.performed += FollowActionOnperformed;
                followAction.Enable();
                
                
                mediAction = new InputAction(
                    name: "MediAction",
                    type: InputActionType.Button,
                    binding: "<Keyboard>/m"
                );
                mediAction.performed += mediActionOnperformed;
                mediAction.Enable();

                
                searchAction = new InputAction(
                    name: "SearchAction",
                    type: InputActionType.Button,
                    binding: "<Keyboard>/o"
                );
                searchAction.performed += SearchActionOnperformed;
                searchAction.Enable();
            }

            private void SearchActionOnperformed(InputAction.CallbackContext obj)
            {
                SwitchState(stateList[(int)State.Search]);
            }

            private void mediActionOnperformed(InputAction.CallbackContext obj)
            {
                if(isMedi) SwitchState(stateList[(int)State.Medi]);
            }


            private void OnDisable()
            {
                if(followAction != null){ followAction.performed -= FollowActionOnperformed; followAction.Disable();}
                if(holdAction != null) {holdAction.performed -= HoldActionOnperformed; holdAction.Disable();}
                if(mediAction != null) {mediAction.performed -= HoldActionOnperformed; mediAction.Disable();}
                if(searchAction != null) {searchAction.performed -= HoldActionOnperformed; searchAction.Disable();}

            }

            private void FollowActionOnperformed(InputAction.CallbackContext obj)
            {
                SwitchState(stateList[(int)State.Follow]);
            }

            private void HoldActionOnperformed(InputAction.CallbackContext obj)
            {
                SwitchState(stateList[(int)State.Hold]);
            }
            
        #endregion
        
        public override void Awake()
        {
            base.Awake();
            
            SwitchState(stateList[(int)State.Follow]);
        }

        public override void SwitchToHunt()
        {
            if (lastAlive || currentState == stateList[(int)State.Search])
            {
                SwitchState(stateList[(int)State.Hunt]);
            }
        }

        public override void SwitchToLastKnownPosition()
        {
            SwitchState(stateList[(int)State.LastKnowPos]);
        }   

        public override void CheckToSwitchLeader()
        {
            if (_group.Count > 1)   // 2 or more
            {
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
                lastAlive = true;
                SwitchState(stateList[(int)State.Search]);
            }
        }
    }
}
