using System;
using UnityEngine;
using Common;
using Common.Interfaces;
using Factory;
using Weapon;

namespace Player
{
    [RequireComponent(typeof(SensingView), typeof(AttackScript))]
    public class PlayerBrain : CharacterFactory
    {
        [Header("Setup guy")] 
        [SerializeField] private float setMaxHealth = 100f;
        [SerializeField] private float shootDistance = 20f;
        
        [Header("View")]
        [SerializeField] private float setMaxViewingDistance = 40f;
        [SerializeField, Range(0,180)] private float setFov = 170f;
        [SerializeField] private LayerMask targetLayerMask;
        
        [Header("Obstacles")]
        [SerializeField] private LayerMask obstacleLayerMask;
        
        private SensingView _view;
        private AttackScript _weapon;

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
            
            healthMax = setMaxHealth;
            health = setMaxHealth;
            
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

        private void OnEnable()
        {
            GameManager.Instance.allEntities.Add(gameObject);
        }

        private void OnDisable()
        {
            if(GameManager.Instance != null)
                GameManager.Instance.allEntities.Remove(gameObject);
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
                
                // rotate guy towards target within 10 degrees of it 
                // then it can use weapon
                if (angle <= 0.95f ) 
                    RotateObject(toTarget);
                else
                    _weapon.Shoot();
            }
        }

        private void RotateObject(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition.normalized*3;
            var rotation= Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 6f*Time.deltaTime);
        }

        private float AngleToTarget(Vector3 targetPosition)
        {
            return Vector3.Dot(transform.forward, targetPosition.normalized);
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}
