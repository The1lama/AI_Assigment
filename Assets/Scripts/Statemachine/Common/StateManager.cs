using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.AI;
using Factory;
using Statemachine.Enemy;
using Statemachine.Friendly;
using UnityEngine;
using UnityEngine.AI;


namespace Statemachine.Common
{
    interface IStates
    {
        public abstract StateMachineFactory currentState { get; set; }
        public abstract StateMachineFactory[] stateList { get; set; }
        public abstract StateMachineFactory lastState { get; set; }
        
    }
    

    public abstract class StateManager : MonoBehaviour, IStates
    {
        #region Variables
            public abstract StateMachineFactory defaultState { get; set; }
        
            public abstract StateMachineFactory currentState { get; set; }
            public abstract StateMachineFactory[] stateList { get; set; }
            public abstract StateMachineFactory lastState { get; set; }
            
            public abstract Vector3 lastKnownPosition { get; set; }
            
            public abstract float offsetAngle { get; set; }
            public abstract Transform leader { get; set; }
            public abstract bool isLeader { get; set; }
            public abstract List<GameObject> _group { get; set; }  
            public abstract float stopingDistance { get; set; }
            public abstract LayerMask teamLayerMask { get; set; }
            public abstract bool lastAlive { get; set; }

            public abstract NavMeshAgent agent { get; set; }
            public abstract AiWalk walkerAgent { get; set; }
            public virtual AiBrain aiBrain { get; set; }
            
            public bool onTheHunt = false;

            [Header("MediClass")] 
            public abstract bool isMedi { get; set; }
            public abstract float helpRadius  { get; set; }
            public abstract bool onHealingRoute { get; set; }
            public abstract List<GameObject> hurtComrades { get; set; }
            
            public abstract SensingView view  { get; set; }

        #endregion

        public virtual void Awake()
        {
            aiBrain = GetComponent<AiBrain>();
            
            view = GetComponent<SensingView>();
            agent = GetComponent<NavMeshAgent>();

            #region SetUp Walker

            walkerAgent = GetComponent<AiWalk>();
            walkerAgent.stopingDistance = stopingDistance;

            #endregion

            #region SetUp group
            if (gameObject.CompareTag("Friendly"))
                _group = GameManager.Instance.friendlyEntities;
            else if(gameObject.CompareTag("Enemy"))
                _group = GameManager.Instance.enemyEnteties;
            else 
                GetTheGroup(); 
            #endregion
            
        }

        public virtual void SwitchState(StateMachineFactory newState)
        {
            currentState?.OnStateExit(this);
            lastState = currentState;
            
            
            
            currentState = newState;
            currentState?.OnStateEnter(this);
        }

        public virtual void Update()
        {

            if(!lastAlive && !CheckLeaderIsAlive())
                CheckToSwitchLeader();


            if (!onTheHunt && aiBrain.seesEnemy)
            {
                SwitchToHunt();
            }
        }

        public abstract void SwitchToHunt();
        
        public abstract void SwitchToLastKnownPosition();

        public abstract void CheckToSwitchLeader();

        private bool CheckLeaderIsAlive()
        {
            if(isLeader)
                return true;
            return leader != null;
        }

        public virtual void FixedUpdate()
        {
            if (isMedi) IfMedic();
            
            currentState?.OnStateUpdate(this);
            
            ShootTowardEnemy();
        }
        
        private void ShootTowardEnemy()
        {
            if (aiBrain.enemyLastKnowPos == null) return;
            var targetPos = aiBrain.enemyLastKnowPos.transform.position;
        
            // if guy to far away to shoot it should walk closer to target and try again if guy sees target
            var toTarget = (transform.position - targetPos).magnitude;
            if (toTarget >= aiBrain.shootDistance || !aiBrain.hasLos) return;
            var angle = aiBrain.AngleToTarget(aiBrain._weapon.transform.forward, (targetPos - transform.position).normalized);
            if (angle <= 0.98f)
            {
                aiBrain.SetVectorRotateTarget(targetPos);
            }
            else
            {
                aiBrain._weapon.Shoot();
            }
        }

        public abstract void IfMedic();
        
        public List<GameObject> FindHurtComrades()
        {
            hurtComrades.Clear();
            var size = Physics.OverlapSphere(transform.position, helpRadius);
            for (int i = 0; i < size.Length; i++)
            {
                var cHealth = size[i];
                var cHealthComponent =  cHealth.GetComponent<CharacterFactory>();
                if(cHealthComponent == null || !cHealthComponent.CompareTag(this.gameObject.tag)) continue;
                if(cHealthComponent.needsHealth)
                    hurtComrades.Add(cHealthComponent.gameObject);
            }
            return hurtComrades;
        }
        
        

        private void GetTheGroup()
        {
            foreach (var memeber in GameManager.Instance.allEntities.Where(m => m.gameObject.CompareTag(gameObject.tag)))
            {
                var distanceTo = (memeber.transform.position - transform.position).magnitude;
                if(distanceTo < 10f)
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
            
            aiBrain.SetQuaternionRotation(offsetLook);
        }

        private void OnDrawGizmos()
        {
            if(isMedi)
                Gizmos.DrawWireSphere(transform.position, helpRadius);
        }
    }
}
