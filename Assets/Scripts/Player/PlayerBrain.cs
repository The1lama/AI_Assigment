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
                
                _weapon.Shoot();
            }
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}
