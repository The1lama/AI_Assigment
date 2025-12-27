using System;
using System.Collections.Generic;
using Factory;
using Statemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Weapon;

namespace Common.AI
{
    [RequireComponent(typeof(SensingView), typeof(AttackScript), typeof(StateManager))]
    public class AiBrain : CharacterFactory
    {
        [Header("Setup guy")]
        [SerializeField] private float _speed = 5;
        [field: SerializeField] public override float healthMax { get; set; } = 100f;
        [SerializeField] private float shootDistance = 20f;
        public GameObject leader;
        
        
        [Header("View")]
        [SerializeField] private float setMaxViewingDistance = 30f;
        [SerializeField, Range(0,180)] private float setFov = 170f;
        [SerializeField] private LayerMask targetLayerMask;
        private bool seesEnemy = false;
        
        [Header("Obstacles")]
        [SerializeField] private LayerMask obstacleLayerMask;
        
        [Header("Debug")]
        [SerializeField]  private bool debug = true;
        [SerializeField] private bool isInRangeAndSeen;


        private void OnEnable()
        {
            GameManager.Instance.allEntities.Add(this.gameObject);
        }

        private void OnDisable()
        {
            if(GameManager.Instance != null)
                GameManager.Instance.allEntities.Remove(gameObject);
        }


        private SensingView _view;
        private AttackScript _weapon;
        
        #region override Stuff

        public override float health { get; set; }
        public override float viewingDistance { get; set; }
        public override LayerMask layerMask { get; set; }

        public override float speed
        {
            get => _speed;
            set => _speed = value;
        } 

        #endregion
        
        
        private void Awake()
        {
            _view = GetComponent<SensingView>();
            _weapon = GetComponent<AttackScript>();
            
            health = healthMax;
            
            InitializeView();
            
            
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

        
        private void Update()
        {
            
            seesEnemy = TryFindEnemy();
            if (seesEnemy) return;

        }


        private bool TryFindEnemy()
        {
            var enemyHit = Physics.OverlapSphere(transform.position, viewingDistance, layerMask);
            foreach (var hit in enemyHit)
            {
                if(!_view.TrySeeTarget(hit.transform, out Vector3 lastKnownPosition, out bool hasLOS, out float distanceToTarget)) continue;
                
                // if guy to to far away to shoot it should walk closer to target and try again if guy sees target
                if(distanceToTarget > shootDistance) break;
                var toTarget = lastKnownPosition - transform.position;
                var angle = AngleToTarget(toTarget);
                
                // rotate guy towards target within 10 degrees of it 
                // then it can use weapon
                if (angle <= 0.95f ) 
                    RotateObject(toTarget);
                else
                    _weapon.Shoot();
                return true;
            }
            return false;
        }

        private void RotateObject(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition.normalized;
            var rotation= Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 6f*Time.deltaTime);
        }

        private float AngleToTarget(Vector3 targetPosition)
        {
            return Vector3.Dot(transform.forward, targetPosition.normalized);
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
