using System.Collections.Generic;
using Common;
using Common.AI;
using Statemachine.Common;
using Statemachine.Common.States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Statemachine.Friendly
{
    
    public class FriendlyStateManager : StateManager
    {
        internal enum State
        {
            Follow,
            Hold,
            Search,
            Medi,
        }

        [Header("States")]
        public override StateMachineFactory currentState { get; set; }

        public override StateMachineFactory[] stateList { get; set; } = new StateMachineFactory[]
        {
            new FollowState(),
            new HoldState(),
            new SearchState(),
            new MediState(),
        };
        public override StateMachineFactory lastState { get; set; }


        [Header("Squad")]
        [field: SerializeField] public override float offsetAngle { get; set; } = 30f;
        public override Transform leader { get; set; }
        public override List<GameObject> _group { get; set; }
        [field:SerializeField] public override float stopingDistance { get; set; } = 3f;
        public override LayerMask teamLayerMask { get; set; }
        public override bool lastAlive { get; set; } = false;
        
        [Header("Movement & Senses")]
        public override NavMeshAgent agent { get; set; }
        public override AiWalk walkerAgent { get; set; }
        public override SensingView view { get; set; }

        [Header("Medic Class")]
        [field:SerializeField] public override bool isMedi { get; set; }

        [field:SerializeField] public override float helpRadius { get; set; } = 7f;
        public override bool onHealingRoute { get; set; } = false;
        public override List<GameObject> hurtComrades { get; set; } = new List<GameObject>();


        public override void Awake()
        {
            base.Awake();
            
            SwitchState(stateList[(int)State.Follow]);
        }


        public override void IfMedic()
        {
            if(!onHealingRoute && FindHurtComrades().Count > 0)
                SwitchState(stateList[(int)State.Medi]);
        }
        
        
        
        
        [Header("Inputs")]
        private InputAction holdAction;
        private InputAction followAction;
        public InputAction mediAction { get; set; }
        private InputAction searchAction;

        #region KeyBinds

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
        

    }
}
