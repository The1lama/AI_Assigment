using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Lab3_Steering_Swarm.Scripts.AI;
using Factory;
using Statemachine.Friendly.States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Statemachine.Friendly
{
    public enum State
    {
        Follow,
        Hold,
        Search,
        Medi,
    }
    
    public class StateManager : MonoBehaviour
    {

        [Header("Squad")]
        [field: SerializeField] public float offsetAngle { get; set; } = 30f;
        public Transform leader;
        public List<GameObject> _group;
        public float separationDistance = 3f;
        public LayerMask teamLayerMask;
        private bool lastAlive = false;
        
        internal NavMeshAgent agent;
        internal SteeringAgent steeringAgent;
        internal CharacterFactory aiBrain;

        
        [Header("MediClass")]
        public bool isMedi = false;
        public float helpRadius = 7f;
        public bool onHealingRoute = false;
        internal List<GameObject> hurtComrades = new List<GameObject>();

        public StateMachineFactory[] stateList = new StateMachineFactory[] {
            new FollowState(),
            new HoldState(),
            new SearchState(),
            new MediState(),
        };
        private StateMachineFactory _currentState;
        internal StateMachineFactory lastState;
        internal SensingView view;
        
        
        [Header("Inputs")]
        private InputAction holdAction;
        private InputAction followAction;
        public InputAction mediAction { get; set; }
        private InputAction searchAction;

        
        public void Awake()
        {
            aiBrain = GetComponent<CharacterFactory>();
            view = GetComponent<SensingView>();

            #region Set group

                if (gameObject.CompareTag("Friendly"))
                    _group = GameManager.Instance.friendlyEntities;
                else if(gameObject.CompareTag("Enemy"))
                    _group = GameManager.Instance.enemyEnteties;
                else 
                    GetTheGroup();

            #endregion
            
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = separationDistance;
            agent.updateRotation = false;
            agent.updatePosition = false;
            agent.radius = separationDistance/3;
            
            steeringAgent = GetComponent<SteeringAgent>();
            
            
            _currentState = stateList[(int)State.Follow];
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
        

        internal void SwitchState(StateMachineFactory newState)
        {
            _currentState.OnStateExit(this);
            lastState = _currentState;
            _currentState = newState;
            _currentState.OnStateEnter(this);
        }


        private void Update()
        {
            if(lastAlive || CheckLeaderIsAlive()) return;

            if (_group.Count > 1)
            {
                if (isMedi)
                {
                    foreach (var comradeAlive in _group)
                    {
                        if (comradeAlive == gameObject ||comradeAlive == null) continue;
                        leader =  comradeAlive.transform;
                        break;
                    }
                }
                else
                {
                    if(_currentState != stateList[(int)State.Search])
                        SwitchState(stateList[(int)State.Search]);
                }
            }
            else
            {
                lastAlive = true;
                if(_currentState != stateList[(int)State.Search])
                    SwitchState(stateList[(int)State.Search]);
            }
        }

        private bool CheckLeaderIsAlive()
        {
            if (leader == null) return false;
            
            return true;
        }
        

        private void FixedUpdate()
        {
            if(isMedi && !onHealingRoute && FindHurtComrades().Count > 0) SwitchState(stateList[(int)State.Medi]);
            
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
        
        
        internal void UpdateAgent(float stopingDistance)
        {
            agent.updateRotation = !agent.updateRotation;
            agent.updatePosition = !agent.updatePosition;
            agent.stoppingDistance = stopingDistance;
        }
        
        
        
    }
}
