using System;
using System.Collections.Generic;
using System.Linq;
using Common.Interfaces;
using Common.Lab3_Steering_Swarm.Scripts.AI;
using Factory;
using Statemachine.States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Statemachine
{
    public enum State
    {
        follow,
        hold,
        medi,
    }
    
    public class StateManager : MonoBehaviour
    {

        [Header("Squad")]
        [field: SerializeField] public float offsetAngle { get; set; } = 30f;
        public Transform leader;
        public List<GameObject> _group;
        public float separationDistance = 3f;
        public LayerMask teamLayerMask;
        
        internal NavMeshAgent agent;
        internal  SteeringAgent steeringAgent;

        
        [Header("MediClass")]
        public bool isMedi = false;
        public float helpRadius = 7f;
        public bool onHealingRoute = false;
        internal List<GameObject> hurtComrades = new List<GameObject>();

        public StateMachineFactory[] stateList = new StateMachineFactory[] {
            new FollowState(),
            new HoldState(),
            new MediState(),
        };
        private StateMachineFactory _currentState;
        internal StateMachineFactory lastState;
        
        
        [Header("Inputs")]
        private InputAction holdAction;
        private InputAction followAction;
        public InputAction mediAction { get; set; }

        
        public void Awake()
        {
            
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = separationDistance;
            agent.updateRotation = false;
            agent.updatePosition = false;
            agent.radius = separationDistance/3;
            
            steeringAgent = GetComponent<SteeringAgent>();
            
            GetTheGroup();
            
            _currentState = stateList[(int)State.follow];
        }


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

                
            }

            private void mediActionOnperformed(InputAction.CallbackContext obj)
            {
                if(isMedi) SwitchState(stateList[(int)State.medi]);
            }


            private void OnDisable()
            {
                if(followAction != null){ followAction.performed -= FollowActionOnperformed; followAction.Disable();}
                if(holdAction != null) {holdAction.performed -= HoldActionOnperformed; holdAction.Disable();}
                if(mediAction != null) {mediAction.performed -= HoldActionOnperformed; mediAction.Disable();}

            }

            private void FollowActionOnperformed(InputAction.CallbackContext obj)
            {
                SwitchState(stateList[(int)State.follow]);
            }

            private void HoldActionOnperformed(InputAction.CallbackContext obj)
            {
                SwitchState(stateList[(int)State.hold]);
            }
            
        #endregion
        

        internal void SwitchState(StateMachineFactory newState)
        {
            _currentState.OnStateExit(this);
            lastState = _currentState;
            _currentState = newState;
            _currentState.OnStateEnter(this);
        }


        private void FixedUpdate()
        {
            if(isMedi && !onHealingRoute && FindHurtComrades().Count > 0) SwitchState(stateList[(int)State.medi]);
            
            _currentState?.OnStateUpdate(this);
        }

        private List<GameObject> FindHurtComrades()
        {
            hurtComrades.Clear();// not working right now
            var amountOfHurtComrades = Physics.OverlapSphere(transform.position, helpRadius, teamLayerMask);
            for (int i = 0; i < amountOfHurtComrades.Length; i++)
            {
                var cHealth = amountOfHurtComrades[i].GetComponent<CharacterFactory>();
                if (cHealth != null && cHealth.needsHealth && amountOfHurtComrades[i].CompareTag(this.gameObject.tag))
                {
                    hurtComrades.Add(cHealth.gameObject);
                }
            }

            return hurtComrades;
        }
        
        

        private void GetTheGroup()
        {
            foreach (var memeber in GameManager.Instance.allEntities.Where(m => m.gameObject.CompareTag(gameObject.tag)))
            {
                _group.Add(memeber.gameObject);
            }
        }
       

        public void SetAgentDestination(Vector3 destination)
        {
            agent.SetDestination(destination);
        }
        
        
        
        public void RotateOffsetFromLeader()
        {
            var dotProd = Vector3.Dot(leader.transform.right, (transform.position - leader.transform.position).normalized);
            var leftOrRight = 1;
            
            if(dotProd < 0 )
                leftOrRight = -1;
            
            var offsetLook = leader.transform.rotation * Quaternion.Euler(0f, offsetAngle*leftOrRight, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, offsetLook, Time.deltaTime *3f);
        }
    }
}
