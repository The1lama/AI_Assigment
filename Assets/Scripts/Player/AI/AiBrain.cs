using System;
using Common;
using Factory;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace Player.AI
{
    [RequireComponent(typeof(NavMeshAgent), typeof(SensingView), typeof(AttackScript))]
    public class AiBrain : CharacterFactory
    {
        [Header("Setup guy")]
        [SerializeField] private float speed = 5;
        [SerializeField] private float maxHealth = 100;
        [SerializeField] private float shootDistance = 20f;
        
        [Header("View")]
        [SerializeField] private float setMaxViewingDistance = 30f;
        [SerializeField, Range(0,180)] private float setFov = 170f;
        [SerializeField] private LayerMask targetLayerMask;
        
        [Header("Obstacles")]
        [SerializeField] private LayerMask obstacleLayerMask;
        
        [Header("Debug")]
        [SerializeField]  private bool debug = true;
        [SerializeField] private bool isInRangeAndSeen;
        
        private SensingView _view;
        private AttackScript _weapon;
        private NavMeshAgent _agent;
        
        #region override Stuff

        public override float health { get; set; }
        public override float viewingDistance { get; set; }
        public override float healthMax { get; set; }
        public override LayerMask layerMask { get; set; }
        
        #endregion
        
        
        private void Awake()
        {
            _view = GetComponent<SensingView>();
            _weapon = GetComponent<AttackScript>();
            
            healthMax = maxHealth;
            health = maxHealth;
            
            InitializeView();
            InitializeAgent();
            
        }

        private void InitializeView()
        {
            viewingDistance = setMaxViewingDistance;
            layerMask = targetLayerMask;
            
            _view.targetLayer = layerMask;
            _view.viewingDistance = viewingDistance;
            _view.fov = setFov;
            _view.obstructionLayerMask = obstacleLayerMask;
        }

        private void InitializeAgent()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;
        }
        
        
        
        
        private void Update()
        {
            TryFindEnemy();
        }

        private void TryFindEnemy()
        {
            var enemyHit = Physics.OverlapSphere(transform.position, viewingDistance, layerMask);
            foreach (var hit in enemyHit)
            {
                if(!_view.TrySeeTarget(hit.transform, out Vector3 lastKnownPosition, out bool hasLOS, out float distanceToTarget)) continue;
                
                // if guy to to far away to shoot it should walk closer to target and try again if guy sees target
                if(distanceToTarget > shootDistance) break;
                var toTarget = lastKnownPosition - transform.position;
                var angle = AngleToTarget(toTarget);
                
                // rotate guy towards target within a 6degreas of it 
                // then it can use weapon
                if (angle <= 0.97f ) 
                    RotateObject(toTarget);
                else
                    _weapon.Shoot();
            }
        }

        private void RotateObject(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition.normalized;
            var rotation= Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10*Time.deltaTime);
        }

        private float AngleToTarget(Vector3 targetPosition)
        {
            return Vector3.Dot(transform.forward, targetPosition.normalized);
        }
        
        public void SetDestination(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }

        private void OnDrawGizmosSelected()
        {
            if (debug)
            {
                Gizmos.color = isInRangeAndSeen ? Color.green : Color.red;
                Gizmos.DrawWireSphere(transform.position, viewingDistance);
                
                Vector3 rightBoundary = Quaternion.Euler(0, setFov * 0.5f, 0) * transform.forward;
                Vector3 leftBoundary = Quaternion.Euler(0, -setFov * 0.5f, 0) * transform.forward;

                // gets shows wrong direction when facing -z dont care enough to fix right now.
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewingDistance);
                Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewingDistance);
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, shootDistance);
            }
        }
    }
}
