using System;
using System.Collections.Generic;
using System.Linq;
using Common.Lab3_Steering_Swarm.Scripts.AI;
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
    }
    
    
    public class StateManager : MonoBehaviour
    {
        private StateMachineFactory _currentState;

        [Header("Squad")]
        [field: SerializeField] public float offsetAngle { get; set; } = 30f;
        public Transform leader;
        public List<GameObject> _group;
        public float separationDistance = 3f;
        [HideInInspector] public NavMeshAgent agent;
        

        public  SteeringAgent steeringAgent;

        public StateMachineFactory[] stateList = new StateMachineFactory[] {
            new FollowState(),
            new HoldState(),
        };
        
        
        [Header("Inputs")]
        private InputAction holdAction;
        private InputAction followAction;
        
        
        

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
                

                
            }

            private void OnDisable()
            {
                if(followAction != null){ followAction.performed -= FollowActionOnperformed; followAction.Disable();}
                if(holdAction != null) {holdAction.performed -= HoldActionOnperformed; holdAction.Disable();}
                

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
        

        private void SwitchState(StateMachineFactory newState)
        {
            Debug.Log($"SwitchState to {newState}");
            
            _currentState.OnStateExit(this);
           
            _currentState = newState;
            _currentState.OnStateEnter(this);
        }


        private void FixedUpdate()
        {
            _currentState?.OnStateUpdate(this);
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
