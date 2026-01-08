using System.Collections;
using Common.AI;
using UnityEngine;

namespace Statemachine.Common.State
{
    public class LastKnownPosState : StateMachineFactory
    {
        
        private AiBrain brain;
        private float shootDistance;
        private Vector3 targetPos;


        private enum SearchPhase
        {
            Moving,
            LookRight,
            LookLeft,
            Done,
        }
        
        private SearchPhase _searchPhase;
        private float _phaseTimer;
        
        
        public override void OnStateEnter(StateManager me)
        {
            brain = me.aiBrain;
            targetPos = me.lastKnownPosition;
            
            _searchPhase = SearchPhase.Moving;
            _phaseTimer = 0f;
        }
        
        public override void OnStateUpdate(StateManager me)
        {
            if (brain.hasLos)
            {
                me.SwitchState(me.lastState);
                return;
            }
            
            switch (_searchPhase)
            {
                case SearchPhase.Moving:
                    if(!me.agent.pathPending)
                        me.walkerAgent.SetDestination(targetPos);
                    if (!me.agent.pathPending && me.agent.remainingDistance <= 0.5f)
                    {
                        _searchPhase = SearchPhase.LookRight;
                        _phaseTimer = 1.5f;
                        
                        var rightLook = me.transform.rotation * Quaternion.Euler(0f, 90f, 0f);
                        brain.SetQuaternionRotation(rightLook);
                    }
                    break;
                case SearchPhase.LookRight:
                    _phaseTimer -= Time.deltaTime;
                    if (_phaseTimer <= 0f)
                    {
                        _searchPhase = SearchPhase.LookLeft;
                        _phaseTimer = 1.5f;
                        
                        var leftLook = me.transform.rotation * Quaternion.Euler(0f, -180f, 0f);
                        brain.SetQuaternionRotation(leftLook);
                    }
                    break;
                
                case SearchPhase.LookLeft:
                    _phaseTimer -= Time.deltaTime;
                    if (_phaseTimer <= 0f)
                    {
                        _searchPhase = SearchPhase.Done;
                    }
                    break;
                case SearchPhase.Done:
                default:
                    me.SwitchState(me.defaultState);
                    break;
                    
                
            }
        }

        public override void OnStateExit(StateManager me)
        {
            me.lastKnownPosition = Vector3.zero;
            
            
        }
    }
    
}
