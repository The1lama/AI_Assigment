using Factory;
using Statemachine;
using UnityEngine;
using UnityEngine.AI;


namespace Statemachine.States
{
    public class MediState : StateMachineFactory
    {
        private ParticleSystem healingParticles;
        private GameObject healingTarget;
        private Vector3 _lastPosition;
        private bool isHealing;
        
        public override void OnStateEnter(StateManager me)
        {
            _lastPosition = me.transform.position;
            
            if(healingParticles == null)
                healingParticles = me.GetComponentInChildren<ParticleSystem>();
            me.onHealingRoute =  true;
            
            healingTarget = me.hurtComrades?[0];
            
            UpdateAgent(me, 1f);
        }

        private void UpdateAgent(StateManager manager, float stopingDistance)
        {
            manager.agent.updateRotation = !manager.agent.updateRotation;
            manager.agent.updatePosition = !manager.agent.updatePosition;
            manager.agent.stoppingDistance = stopingDistance;
        }
        
        

        public override void OnStateUpdate(StateManager me)
        {
            #region null and list checks

                if (me.hurtComrades.Count <= 0)
                {
                    me.SwitchState(me.lastState);
                }
                
                if (healingTarget == null)
                {
                    if(MabyeSwitchHealingTarget(me)) return;
                }
                
            #endregion
            
            
            var comrade = healingTarget.GetComponent<CharacterFactory>();

            // Checks if dddcomrade needs healing switches out for a new target if list is more than 1
            if (!comrade.needsHealth)
            {
                if (MabyeSwitchHealingTarget(me)) return;
            }

            me.agent.SetDestination(healingTarget.transform.position);
            
            var distanceToComrade = Vector3.Distance(me.transform.position, healingTarget.transform.position);
            if (distanceToComrade < 2.5)
            {
                healingParticles.transform.position = healingTarget.transform.position;
                if (!isHealing)
                {
                    healingParticles?.Play();
                    isHealing = true;
                }
                comrade.TakeHeal(10f);
            }
            else
            {
                healingParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                isHealing = false;
            }
        }

        private bool MabyeSwitchHealingTarget(StateManager me)
        {
            if (me.hurtComrades.Count < 2)
            {
                me.SwitchState(me.lastState);
                return true;
            }
                
            me.hurtComrades.RemoveAt(0);
            healingTarget =  me.hurtComrades[0];
            return false;
        }

        public override void OnStateExit(StateManager me)
        {
            me.agent.SetDestination(_lastPosition);
            me.onHealingRoute = false;
            UpdateAgent(me, me.separationDistance);
        }
    }
}
