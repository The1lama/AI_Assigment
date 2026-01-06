using UnityEngine;

namespace Statemachine.Friendly.States
{
    public class HoldState : StateMachineFactory
    {
        private Transform currentPosition;

        private Quaternion startRotation;
        private bool isDone = false;
        float offestAngle = 90f;
        private int currentRotationIndex = 0;

        private float[] angles = new float[]
        {
            90f,
            -90f,
        };
    
    
        public override void OnStateEnter(FriendlyStateManager me)
        {
            currentPosition = me.transform;
            me.agent.SetDestination(currentPosition.position);
            me.agent.stoppingDistance = 0f;
            me.agent.autoBraking = false;

            startRotation = me.transform.rotation;
            currentRotationIndex = Random.Range(0, angles.Length);
        }

        public override void OnStateUpdate(FriendlyStateManager me)
        {
            //TODO: Make the agent look around

            var targetRotation = startRotation * Quaternion.Euler(0f, angles[currentRotationIndex], 0f);
            me.transform.rotation = Quaternion.Slerp(me.agent.transform.rotation, targetRotation, 1.5f*Time.deltaTime);


            if (targetRotation != me.transform.rotation) return; // does not work with 180 degrees but works with 90 
            currentRotationIndex = ( currentRotationIndex += 1 ) % angles.Length; 
            startRotation = me.transform.rotation;

        }

        public override void OnStateExit(FriendlyStateManager me)
        {
            me.agent.stoppingDistance = me.stopingDistance;
            me.agent.autoBraking = true;
        }
    }
}
