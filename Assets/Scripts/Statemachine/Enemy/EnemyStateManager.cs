using System.Collections.Generic;
using System.Linq;
using Common;
using Common.AI;
using Factory;
using Statemachine.Enemy.States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Statemachine.Enemy
{
    
    public class EnemyStateManager : MonoBehaviour
    {
        internal enum State
        {
            Waypoint,
            Follow,
            Hold,
            Search,
            Medi,
        }

        [Header("Squad")]
        [field: SerializeField] public float offsetAngle { get; set; } = 30f;
        public Transform leader;
        public List<GameObject> _group;
        public float stopingDistance = 3f;
        public LayerMask teamLayerMask;
        private bool lastAlive = false;
        
        internal NavMeshAgent agent;
        internal AiWalk walkerAgent;
        internal CharacterFactory aiBrain;
        
        [Header("MediClass")]
        public bool isMedi = false;
        public float helpRadius = 7f;
        public bool onHealingRoute = false;
        internal List<GameObject> hurtComrades = new List<GameObject>();

        public EnemyStateMachineFactory[] stateList = new EnemyStateMachineFactory[] {
            new WaypointState(),
            new FollowState(),
            new HoldState(),
            new SearchState(),
            new MediState(),
        };
        private EnemyStateMachineFactory _currentEnemyState;
        internal EnemyStateMachineFactory lastState;
        internal SensingView view;
        
        
        [Header("Inputs")]
        private InputAction waypointAction;

        
        public void Awake()
        {
            aiBrain = GetComponent<CharacterFactory>();
            view = GetComponent<SensingView>();
            agent = GetComponent<NavMeshAgent>();
            
            
            #region Setup Walker
            
            walkerAgent = GetComponent<AiWalk>();
            walkerAgent.stopingDistance = stopingDistance;

            #endregion

            #region Set group

                if (gameObject.CompareTag("Friendly"))
                    _group = GameManager.Instance.friendlyEntities;
                else if(gameObject.CompareTag("Enemy"))
                    _group = GameManager.Instance.enemyEnteties;
                else 
                    GetTheGroup();

            #endregion
            
            
            SwitchState(stateList[(int)State.Search]);
        }

        #region KeyBinds

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

        internal void SwitchState(EnemyStateMachineFactory newEnemyState)
        {
            _currentEnemyState.OnStateExit(this);
            lastState = _currentEnemyState;
            _currentEnemyState = newEnemyState;
            _currentEnemyState.OnStateEnter(this);
        }


        private void Update()
        {
            if(lastAlive || CheckLeaderIsAlive()) return;   // Returns if not is lastalive/ leader is alive

            if (_group.Count > 1)   // 2 or more
            {
                if (isMedi)     // searches for hurt comrades
                {
                    // gets a new leader thats not a medic
                    foreach (var comradeAlive in _group)
                    {
                        if (comradeAlive == gameObject ||comradeAlive == null) continue;
                        leader =  comradeAlive.transform;
                        break;
                    }
                }
           //     else
           //     {
           //         if(_currentEnemyState != stateList[(int)State.Search])
           //             SwitchState(stateList[(int)State.Search]);
           //     }
            }
           // else  // only one left
           // {
           //     lastAlive = true;
           //     if(_currentState != stateList[(int)State.Search])
           //         SwitchState(stateList[(int)State.Search]);
           // }
        }

        private bool CheckLeaderIsAlive()
        {
            if (leader == null) return false;
            
            return true;
        }
        

        private void FixedUpdate()
        {
            if(isMedi && !onHealingRoute && FindHurtComrades().Count > 0) SwitchState(stateList[(int)State.Medi]);
            
            _currentEnemyState?.OnStateUpdate(this);
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
