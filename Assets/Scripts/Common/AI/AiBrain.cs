using Factory;
using Statemachine.Common;
using UnityEngine;
using Weapon;

namespace Common.AI
{
    [RequireComponent(typeof(SensingView), typeof(AttackScript), typeof(AiWalk))]
    public class AiBrain : CharacterFactory
    {
        
        [Header("Setup guy")]
        [SerializeField] private float _speed = 5;
        [field: SerializeField] public override float healthMax { get; set; } = 100f;
        public float shootDistance = 20f;
        public GameObject leader;
        
        
        [Header("View")]
        [SerializeField] private float setMaxViewingDistance = 30f;
        [SerializeField, Range(0,180)] private float setFov = 170f;
        [SerializeField] private LayerMask targetLayerMask;
        public bool seesEnemy = false;
        public Vector3 enemyLastKnowPos;
        public bool hasLos;
        
        [Header("Obstacles")]
        [SerializeField] private LayerMask obstacleLayerMask;


        private StateManager _stateManager;
        [Header("Squad")]
        public float offsetAngle = 30f;
        public float stopingDistance = 3f;

        [Header("Medic Class")] 
        public bool isMedi = false;
        public float helpRadius = 7f;
        
        [Header("Debug")]
        [SerializeField] private bool debug = true;
        [SerializeField] private bool isInRangeAndSeen;


        private void OnEnable()
        {
            GameManager.Instance.allEntities.Add(this.gameObject);
            
            if(gameObject.CompareTag("Friendly"))
                GameManager.Instance.friendlyEntities.Add(this.gameObject);
            else if(gameObject.CompareTag("Enemy"))
                GameManager.Instance.enemyEnteties.Add(this.gameObject);
            
        }

        private void OnDisable()
        {
            if (GameManager.Instance == null) return; 
            
            GameManager.Instance.allEntities.Remove(gameObject);
            
            if(gameObject.CompareTag("Friendly"))
                GameManager.Instance.friendlyEntities.Remove(this.gameObject);
            else if(gameObject.CompareTag("Enemy"))
                GameManager.Instance.enemyEnteties.Remove(this.gameObject);
            
            
        }


        private SensingView _view;
        public AttackScript _weapon;
        
        #region override Stuff

        public override float health { get; set; }

        public override float viewingDistance
        {
            get => setMaxViewingDistance; 
            set => setMaxViewingDistance = value;
        }
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

            InitializeStateMachine();

        }

        private void InitializeStateMachine()
        {
            _stateManager = GetComponent<StateManager>();

            if (_stateManager == null)
            {
                Debug.LogWarning("State machine not found");
                return;
            }
            
            _stateManager.leader = leader.transform;
            
            _stateManager.offsetAngle = offsetAngle;
            _stateManager.stopingDistance = stopingDistance;
            
            _stateManager.isMedi = isMedi;
            _stateManager.helpRadius = helpRadius;

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
                
                seesEnemy = true;
                enemyLastKnowPos = lastKnownPosition;
                hasLos = hasLOS;
            }
            seesEnemy =  false;
            hasLos = false;
        }

        public void RotateObject(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition.normalized;
            var rotation= Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1f*Time.deltaTime);
        }

        public float AngleToTarget(Vector3 targetPosition)
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
