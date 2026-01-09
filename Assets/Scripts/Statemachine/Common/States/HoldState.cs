using UnityEngine;

namespace Statemachine.Common.States
{
    public class HoldState : StateMachineFactory
    {
        private Transform currentPosition;

        private Quaternion startRotation;
        private int currentRotationIndex = 0;
        private float _phaseTimer;

        private float[] angles = new float[]
        {
            90f,
            270f,
            -90f,
            23f,
        };
    
    
        public override void OnStateEnter(StateManager me)
        {
            currentPosition = me.transform;
            
            me.walkerAgent?.SetDestination(currentPosition.position);
            me.agent.stoppingDistance = 0f;
            
            _phaseTimer = 0f;
            me.aiBrain.rotateOverride = !me.aiBrain.rotateOverride;

            startRotation = me.transform.rotation;
            currentRotationIndex = Random.Range(0, angles.Length);
        }

        public override void OnStateUpdate(StateManager me)
        {
             _phaseTimer -= Time.deltaTime;
            if (_phaseTimer <= 0f)
            {
                _phaseTimer = 2f;
                
                var leftLook = me.transform.rotation * Quaternion.Euler(0f, angles[currentRotationIndex], 0f);
                me.aiBrain.SetQuaternionRotation(leftLook);
                
                currentRotationIndex = ( currentRotationIndex += 1 ) % angles.Length; 
            }

        }

        public override void OnStateExit(StateManager me)
        {
            me.agent.stoppingDistance = me.stopingDistance;
        }
    }
}
